using DIT.Application.Interfaces;
using DIT.Core;
using DIT.Core.Dtos;
using DIT.Core.Entities;
using DIT.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using WebApi.Helpers;

namespace DIT.Controllers
{
    //[Authorize]
    public class ProductController : ControllerBase
    {
        #region ===[ Private Members ]=============================================================

        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region ===[ Constructor ]=================================================================

        /// <summary>
        /// Initialize ContactController by injecting an object type of IUnitOfWork
        /// </summary>
        public ProductController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        #endregion

        #region ===[ Public Methods ]==============================================================

        [HttpGet("/api/v1/product")]
        public async Task<ActionResult<GetResponse<Product>>> GetAll(GetRequest request)
        {
			GetResponse<Product> apiResponse = new GetResponse<Product>();

			try
			{
				apiResponse = await _unitOfWork.Product.GetAllAsync(request.category, request.page, request.size, request.q);

				if(apiResponse is null)
				{
					return BadRequest();
				}
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

        [HttpGet("/api/v1/product/{id}")]
		public async Task<Product> GetById(Guid id)
		{

			var apiResponse = new Product();
			try
			{
				apiResponse = await _unitOfWork.Product.GetByIdAsync(id);
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

        [HttpPost("/api/v1/product")]
        public async Task InsertOrUpdate([FromBody] PostProductRequest request)
        {

            try
            {
                await _unitOfWork.Product.InsertOrUpdateAsync(request);
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

        [HttpDelete("/api/v1/product/delete/{id}")]
        public async Task DeleteById(Guid id)
        {

            try
            {
                await _unitOfWork.Product.DeleteByIdAsync(id);
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
