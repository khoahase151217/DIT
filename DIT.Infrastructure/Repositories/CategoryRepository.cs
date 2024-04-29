using Dapper;
using DIT.Application.Interfaces;
using DIT.Core.Entities;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

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

		public async Task<IEnumerable<Category>> GetAllAsync()
		{
			using (IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DBConnection")))
			{
				connection.Open();

				var queries = "SELECT [ID],[CategoryName],[Photo] FROM [Categories] (NOLOCK);";

				var result = await connection.QueryAsync<Category>(queries);
				return result.ToList();
			}
		}

		#endregion
	}
}
