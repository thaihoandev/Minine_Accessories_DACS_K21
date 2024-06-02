using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Accessories_Store.Models.Entities;

public partial class Transaction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [ForeignKey("Order")]
    public long? OrderId { get; set; }

    [ForeignKey("ApplicationUser")]
    public string? UserId { get; set; }
    public double? Price { get; set; }

    public int? Quantity { get; set; }


    public int? TotalPrice { get; set; }

    public string? Name { get; set; }

    public int? Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public Order Order { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

}
