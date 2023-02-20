using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebAppCmvc.Models
{
    public class ProdCatg
    {
        public Product prod { get; set; }
        public List<SelectListItem> catgs { get; set; }

        public ProdCatg()
        {
            prod = new Product();
            catgs = new List<SelectListItem>();
        }
    }
}
