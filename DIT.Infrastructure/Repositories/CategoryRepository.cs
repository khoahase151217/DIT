using Dapper;
using DIT.Application.Interfaces;
using DIT.Core.Dtos;
using DIT.Core.Entities;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DIT.Infrastructure
{
    public class CategoryRepository : ICategoryRepository
    {
        #region ===[ Private Members ]=============================================================

        private readonly IConfiguration _configuration;

        #endregion

        #region ===[ Constructor ]=================================================================

        public CategoryRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        #endregion

        #region ===[ IContactRepository Methods ]==================================================

        public async Task<IEnumerable<Category>> GetAllAsync(string q)
        {
            using (IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DBConnection")))
            {
                connection.Open();

                string whereStatement = string.Empty;

                if (!string.IsNullOrWhiteSpace(q))
                {
                    string[] keyArr = q.Split(' ');
                    for (int i = 0; i < keyArr.Length; i++)
                    {
                        if (i == 0)
                        {
                            whereStatement += $"WHERE [CategoryName] LIKE N'%{keyArr[i]}%' ";
                            continue;
                        }

                        whereStatement += $"AND [CategoryName] LIKE N'%{keyArr[i]}%'";

                    }
                }

                var queries = @$"SELECT [ID],[CategoryName],[Photo] FROM [Categories] (NOLOCK){whereStatement};";

                var result = await connection.QueryAsync<Category>(queries);
                return result.ToList();
            }
        }

        public async Task<Category> GetByIdAsync(Guid id)
        {
            using (IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DBConnection")))
            {
                connection.Open();

                string whereStatement = $"WHERE [ID] = '{id}'";

                var selectQueries = @$"
                SELECT
                    [ID],
                    [CategoryName],
                    [Photo] FROM [Categories] (NOLOCK)
                {whereStatement}                
                ;";

                var result = await connection.QuerySingleOrDefaultAsync<Category>(selectQueries);

                return result;
            }
        }

        public async Task DeleteByIdAsync(Guid id)
        {
            using (IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DBConnection")))
            {
                connection.Open();

                var sql = @$"DELETE FROM [Categories] WHERE [ID] = '{id}'";

                await connection.ExecuteAsync(sql);
            }
        }

        public async Task InsertOrUpdateAsync(PostCategoryRequest request)
        {
            using (IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DBConnection")))
            {
                connection.Open();

                if (request.ID != null)
                {

                    var selectQueries = @$"
                    SELECT
                        [ID],
                        [CategoryName],
                        [Photo] FROM [Categories] (NOLOCK)
                    WHERE [ID] = '{request.ID}'              
                    ;";

                    var category = await connection.QuerySingleOrDefaultAsync<Category>(selectQueries);

                    if (request.CategoryName != null)
                    {
                        category.CategoryName = request.CategoryName;
                    }

                    if (request.Photo != null)
                    {
                        category.Photo = Convert.FromBase64String(request.Photo);
                    }

                    string sql = "UPDATE Categories SET CategoryName = @CategoryName, Photo = @Photo WHERE ID = @ID;";
                    await connection.ExecuteAsync(sql, category);
                }
                else
                {
                    Guid id = Guid.NewGuid();
                    var sql = "INSERT INTO Categories (ID, CategoryName, Photo) VALUES (@ID, @CategoryName, @Photo)";

                    var category = new Category() { ID = id, CategoryName = request.CategoryName, Photo = Convert.FromBase64String(request.Photo) };
                    await connection.ExecuteAsync(sql, category);

                }

            }
        }

        #endregion
    }
}
