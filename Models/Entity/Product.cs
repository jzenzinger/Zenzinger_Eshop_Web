using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using Microsoft.AspNetCore.Http;
using Zenzinger_Eshop_Web.Models.Validation;

namespace Zenzinger_Eshop_Web.Models.Entity
{
    [Table(nameof(Product))]
    public class Product
    {
        [Key]
        [Required]
        public int ID { get; set; }
        [StringLength(255)]
        [Required]
        public string Name { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }
        [StringLength(255)]
        [Required]
        public string ImageSource { get; set; }
        [StringLength(50)]
        public string ImageAlt { get; set; }
        [Required]
        public double Price { get; set; }
        [StringLength(1024)]
        [Required]
        public string Info { get; set; }
    }
}