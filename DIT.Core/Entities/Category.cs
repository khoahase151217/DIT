using DIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Core.Entities
{
	public class Category : BaseEntity
	{
		public string CategoryName { get; set; }

		public byte[]? Photo { get; set; }

	}
}
