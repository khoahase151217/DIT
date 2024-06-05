using Dapper;
using DIT.Application.Interfaces;
using DIT.Core.Entities;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using DIT.Core;
using System.Text;
using DIT.Core.Dtos;

namespace DIT.Infrastructure
{
	public class ProductRepository : IProductRepository
	{
		#region ===[ Private Members ]=============================================================

		private readonly IConfiguration _configuration;

		#endregion

		#region ===[ Constructor ]=================================================================

		public ProductRepository(IConfiguration configuration)
		{
			this._configuration = configuration;
		}

		#endregion

		#region ===[ IContactRepository Methods ]==================================================

		public async Task<GetResponse<Product>> GetAllAsync(Guid? category, int? page, int? size, string? q)
		{
			using (var connection = new SqlConnection(_configuration.GetConnectionString("DBConnection")))
			{

				await connection.OpenAsync();

				string whereStatement = string.Empty;

				if (category is not null)
				{

					var categoryQueries = @$"
					SELECT
						[ID] FROM [Categories] (NOLOCK)
					WHERE [ID] = '{category}'             
					;";

					var categoryResult = await connection.QuerySingleOrDefaultAsync<Category>(categoryQueries);

					if (categoryResult is null)
					{
						return null;
					}

					whereStatement = $"WHERE [CategoryID] = '{category}'";
				}

				int? pageNumber = page is null ? 0 : page;
				int? pageSize = size is null ? 10 : size;

				if (!string.IsNullOrWhiteSpace(q))
				{

					whereStatement += string.IsNullOrEmpty(whereStatement) ? "WHERE " : " AND ";

					string[] keyArr = q.Split(' ');
					for (int i = 0; i < keyArr.Length; i++)
					{
						if (i == 0)
						{
							whereStatement += $"[ProductName] LIKE N'%{keyArr[i]}%' ";
							continue;
						}

						whereStatement += $"AND [ProductName] LIKE N'%{keyArr[i]}%'";

					}
				}

				// Set first query
				var queries = @$"
					SELECT
                    [Products].[ID],
                    [Products].[ProductName],
                    [Products].[CategoryID],
                    [Products].[Photo],
                    [Products].[ViewCount],
					[Categories].[CategoryName] FROM [Products] (NOLOCK)
					LEFT JOIN [Categories] (NOLOCK)
					ON [Products].[CategoryID] = [Categories].[ID]
                {whereStatement}                
                ORDER BY [ProductName]
                OFFSET @PageSize * @PageNumber ROWS
                FETCH NEXT @PageSize ROWS ONLY;";

				// Set second query, separated with semi-colon
				queries += @$"SELECT COUNT(*) AS TotalItems FROM [Products] (NOLOCK)
                {whereStatement}    
                ";

				// Execute multiple queries with Dapper in just one step
				using var multi = await connection.QueryMultipleAsync(queries,
					new
					{
						PageNumber = pageNumber,
						PageSize = pageSize
					});

				// Fetch Items by OFFSET-FETCH clause
				var items = await multi.ReadAsync<Product>().ConfigureAwait(false);

				// Fetch Total items count
				var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

				// Create paged result
				var result = new GetResponse<Product>()
				{
					content = items,
					last = false,
					totalElements = totalItems,
					pageable = new Pagination()
					{
						pageSize = (int)pageSize,
						pageNumber = (int)pageNumber
					}
				};

				return result;
			}
		}

		public async Task<Product> GetByIdAsync(Guid id)
		{
			using (IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DBConnection")))
			{
				connection.Open();

				string whereStatement = $"WHERE [Products].[ID] = '{id}'";

				var selectQueries = @$"
                SELECT
                    [Products].[ID],
                    [Products].[ProductName],
                    [Products].[CategoryID],
                    [Products].[Photo],
                    [Products].[ViewCount],
					[Categories].[CategoryName] FROM [Products] (NOLOCK)
					LEFT JOIN [Categories] (NOLOCK)
					ON [Products].[CategoryID] = [Categories].[ID]
                {whereStatement}                
                ;";

				var result = await connection.QuerySingleOrDefaultAsync<Product>(selectQueries);

				if (result is not null)
				{
					var updateQueries = @$"
					UPDATE [Products] SET [ViewCount] = [ViewCount] + 1
					{whereStatement}                
					;";

					await connection.ExecuteAsync(updateQueries);
				}

				return result;
			}
		}

        public async Task<Product> DeleteByIdAsync(Guid id)
        {
            using (IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DBConnection")))
            {
                connection.Open();

                string whereStatement = $"WHERE [Products].[ID] = '{id}'";

                var selectQueries = @$"
                SELECT
                    [Products].[ID],
                    [Products].[ProductName],
                    [Products].[CategoryID],
                    [Products].[Photo],
                    [Products].[ViewCount],
					[Categories].[CategoryName] FROM [Products] (NOLOCK)
					LEFT JOIN [Categories] (NOLOCK)
					ON [Products].[CategoryID] = [Categories].[ID]
                {whereStatement}                
                ;";

                var product = await connection.QuerySingleOrDefaultAsync<Product>(selectQueries);

                var sql = @$"DELETE FROM [Products] WHERE [ID] = '{id}'";

                await connection.ExecuteAsync(sql);

                return product;
            }
        }

        public async Task InsertOrUpdateAsync(PostProductRequest request)
        {
            using (IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DBConnection")))
            {
                connection.Open();

                if (request.ID != null)
                {

                    var selectQueries = @$"
                    SELECT
						[ID],
						[ProductName],
						[CategoryID],
						[Photo] FROM [Products] (NOLOCK)
                    WHERE [ID] = '{request.ID}'              
                    ;";

                    var product = await connection.QuerySingleOrDefaultAsync<Product>(selectQueries);

                    if (request.ProductName != null)
                    {
                        product.ProductName = request.ProductName;
                    }

                    if (request.CategoryID != null)
                    {
                        product.CategoryID = request.CategoryID;
                    }

                    if (request.Photo != null)
                    {
                        product.Photo = Convert.FromBase64String(request.Photo);
                    }

                    string sql = "UPDATE Products SET ProductName = @ProductName, CategoryID = @CategoryID, Photo = @Photo WHERE ID = @ID;";
                    await connection.ExecuteAsync(sql, product);
                }
                else
                {
                    Guid id = Guid.NewGuid();
                    var sql = "INSERT INTO Products (ID, ProductName, CategoryID, Photo, ViewCount) VALUES (@ID, @ProductName, @CategoryID, @Photo, @ViewCount)";

                    var product = new Product() { ID = id, ProductName = request.ProductName, CategoryID = request.CategoryID, Photo = Convert.FromBase64String(request.Photo), ViewCount = 1 };
                    await connection.ExecuteAsync(sql, product);

                }

            }
        }
        #endregion
    }
}
