using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zenzinger_Eshop_Web.Models.Entity.Identity;

namespace Zenzinger_Eshop_Web.Models.Entity
{
    [Table(nameof(Order))]
    public class Order : IEnumerable
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateTimeCreated { get; protected set; }

        [StringLength(25)]
        [Required]
        public string OrderNumber { get; set; }

        [Required]
        public double TotalPrice { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }

        public IList<OrderItem> OrderItems { get; set; }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}