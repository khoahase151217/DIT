using Dapper;
using DIT.Application.Interfaces;
using DIT.Core.Entities;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using DIT.Core;
using System.Text;

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
                    [ID],
                    [ProductName],
                    [CategoryID],
                    [Photo],
                    [ViewCount] FROM [Products] (NOLOCK)
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

				string whereStatement = $"WHERE [ID] = '{id}'";

				var selectQueries = @$"
                SELECT
                    [ID],
                    [ProductName],
                    [CategoryID],
                    [Photo],
                    [ViewCount] FROM [Products] (NOLOCK)
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
		#endregion
	}
}
