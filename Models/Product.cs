using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace WebAppCmvc.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        [AllowNull]
        public string Description { get; set; }
        [AllowNull]
        public string ImagePath { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int Price { get; set; }

        [Required]
        public string Type { get; set; }
    }
}
