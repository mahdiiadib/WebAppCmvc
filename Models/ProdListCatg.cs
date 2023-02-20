using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebAppCmvc.Models
{
    public class ProdListCatg
    {
        public List<Product> prods { get; set; }
        public List<SelectListItem> catgs { get; set; }

        public ProdListCatg()
        {
            prods = new List<Product>();
            catgs = new List<SelectListItem>();
        }
    }
}
