using DIT.Application.Interfaces;
using DIT.Core;
using DIT.Core.Dtos;
using DIT.Core.Entities;
using DIT.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Data.SqlClient;
using WebApi.Helpers;

namespace DIT.Api.Controllers
{
    //[Authorize]
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
		public async Task<List<Category>> GetAll(GetCategoryRequest request)
		{
			List<Category> apiResponse = new List<Category>();
			try
			{
				apiResponse = (List<Category>)await _unitOfWork.Category.GetAllAsync(request.q);
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

        [HttpGet("/api/v1/category/{id}")]
        public async Task<Category> GetById(Guid id)
        {

            var apiResponse = new Category();
            try
            {
                apiResponse = await _unitOfWork.Category.GetByIdAsync(id);
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

        [HttpPost("/api/v1/category")]
        public async Task InsertOrUpdate([FromBody] PostCategoryRequest request)
        {

            try
            {
                await _unitOfWork.Category.InsertOrUpdateAsync(request);
            }
            catch (SqlException ex)
            {
                Logger.Instance.Error("SQL Exception:", ex);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Exception:", ex);
            }

        }

        [HttpDelete("/api/v1/category/delete/{id}")]
        public async Task DeleteById(Guid id)
        {

            try
            {
                await _unitOfWork.Category.DeleteByIdAsync(id);
            }
            catch (SqlException ex)
            {
                Logger.Instance.Error("SQL Exception:", ex);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Exception:", ex);
            }
        }

        #endregion
    }
}
