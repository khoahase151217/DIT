using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Core.Dtos
{
    public class PostCategoryRequest
    {
        public Guid? ID { get; set; }
        public string CategoryName { get; set; }

        public string Photo { get; set; }
    }
}
