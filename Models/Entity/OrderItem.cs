using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zenzinger_Eshop_Web.Models.Entity
{
    [Table(nameof(OrderItem))]
    public class OrderItem
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public int Amount { get; set; }
        public double Price { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}