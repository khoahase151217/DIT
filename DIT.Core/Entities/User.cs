using DIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Core.Entities
{
	public class User
	{
		public string UserName { get; set; }

		public string? Password { get; set; }

        public string? Token { get; set; }
    }
}
