using DIT.Core.Dtos;
using DIT.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Data.SqlClient;
using WebApi.Helpers;
using WebApi.Services;

namespace DIT.Api.Controllers
{
	public class AuthenticationController : ControllerBase
	{
		#region ===[ Private Members ]=============================================================

		private readonly IAuthenticationService _authenticationService;

        #endregion

        #region ===[ Constructor ]=================================================================

        /// <summary>
        /// Initialize ContactController by injecting an object type of IAuthenticationService
        /// </summary>
        public AuthenticationController(IAuthenticationService authenticationService)
		{
			this._authenticationService = authenticationService;
		}

		#endregion

		#region ===[ Public Methods ]==============================================================

		[HttpGet("/api/v1/login")]
		public AuthenticateResponse Login(AuthenticateRequest request)
		{
            AuthenticateResponse apiResponse = new AuthenticateResponse(new Core.Entities.User { UserName = string.Empty }, string.Empty);

            try
			{
                apiResponse = (AuthenticateResponse)_authenticationService.Authenticate(request);
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

        [Authorize]
        [HttpGet("/api/v1/getProfile")]
        public AuthenticateResponse GetProfile()
        {
            AuthenticateResponse apiResponse = new AuthenticateResponse(new Core.Entities.User { UserName = string.Empty }, string.Empty);
            try
            {
                apiResponse = (AuthenticateResponse)_authenticationService.GetProfile(Request.Headers.Authorization);
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
