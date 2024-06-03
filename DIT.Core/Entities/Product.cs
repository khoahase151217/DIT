using DIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Core.Entities
{
	public class Product : BaseEntity
	{
		public string ProductName { get; set; }

		public Guid? CategoryID { get; set; }

        public string? CategoryName { get; set; }

        public byte[]? Photo { get; set; }
		public int ViewCount { get; set; }
	}
}
