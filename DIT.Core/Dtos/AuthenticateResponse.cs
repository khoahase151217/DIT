using DIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Core.Dtos
{
    public class AuthenticateResponse
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string? ExpTime { get; set; }


        public AuthenticateResponse(User user, string expTime)
        {

            Username = user.UserName;
            Token = user.Token;
            ExpTime = expTime;
        }
    }
}
