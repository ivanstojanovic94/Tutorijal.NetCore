using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bravissimo.Web.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categorys { get; set; }
    }
}
