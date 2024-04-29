using DIT.Application.Interfaces;
using DIT.Core;
using DIT.Core.Entities;
using DIT.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Data.SqlClient;

namespace DIT.Api.Controllers
{
	public class CategoryController : ControllerBase
	{
		#region ===[ Private Members ]=============================================================

		private readonly IUnitOfWork _unitOfWork;

		#endregion

		#region ===[ Constructor ]=================================================================

		/// <summary>
		/// Initialize ContactController by injecting an object type of IUnitOfWork
		/// </summary>
		public CategoryController(IUnitOfWork unitOfWork)
		{
			this._unitOfWork = unitOfWork;
		}

		#endregion

		#region ===[ Public Methods ]==============================================================

		[HttpGet("/api/v1/category")]
		public async Task<List<Category>> GetAll()
		{
			List<Category> apiResponse = new List<Category>();
			try
			{
				apiResponse = (List<Category>)await _unitOfWork.Category.GetAllAsync();
			}
			catch (SqlException ex)
			{

				Logger.Instance.Error("SQL Exception:", ex);
			}
			catch (Exception ex)
			{

				Logger.Instance.Error("Exception:", ex);
			}

			return apiResponse;
		}

		#endregion
	}
}
