using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using Microsoft.AspNetCore.Http;
using Zenzinger_Eshop_Web.Models.Validation;

namespace Zenzinger_Eshop_Web.Models.Entity
{
    [Table(nameof(CarouselItem))]
    public class CarouselItem
    {
        [Key] [Required] public int id { get; set; }

        [NotMapped] // Nema co delat v databazi
        [FileContentValidation("image")]
        public IFormFile Image { get; set; }

        [StringLength(255)]
        [Required]
        public string ImageSource { get; set; }
        [StringLength(50)]
        public string ImageAlt { get; set; }
    }
}