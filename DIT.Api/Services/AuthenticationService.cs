namespace WebApi.Services;

using DIT.Api.Helpers;
using DIT.Core.Dtos;
using DIT.Core.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public interface IAuthenticationService
{
    AuthenticateResponse Authenticate(AuthenticateRequest model);
    AuthenticateResponse GetProfile(string token);
}

public class AuthenticationService : IAuthenticationService
{

    private readonly AppSettings _appSettings;

    public AuthenticationService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    public AuthenticateResponse Authenticate(AuthenticateRequest model)
    {

        // return null if user not found
        if (!model.Username.Equals(_appSettings.UserName) || !model.Password.Equals(_appSettings.Password)) return null;

        var user = new User
        {
            UserName = _appSettings.UserName
        };

        // authentication successful so generate jwt token
        user.Token = generateJwtToken();

        return new AuthenticateResponse(user);
    }

    public AuthenticateResponse GetProfile(string token)
    {
        var user = new User
        {
            UserName = _appSettings.UserName,
            Token = token
        };
        return new AuthenticateResponse(user);
    }

    // helper methods

    private string generateJwtToken()
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("token", _appSettings.Secret) }),
            Expires = DateTime.UtcNow.AddMinutes(3),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}