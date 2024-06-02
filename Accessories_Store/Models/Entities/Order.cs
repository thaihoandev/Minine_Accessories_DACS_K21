using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Accessories_Store.Models.Entities;

public partial class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [ForeignKey("ApplicationUser")]
    public string? UserId { get; set; }
	public string Name { get; set; }

	public string? Address { get; set; }
	public string? Phone { get; set; }
	public double? Price { get; set; }

    public double? TotalPrice { get; set; }

    public double? TotalDiscount { get; set; }

    public int? OrderStatusPayment { get; set; }

    public int? OrderStatusTransport { get; set; }

    public int? TotalQuantity { get; set; }

    public string? Notes { get; set; }
	
	public int? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public ApplicationUser ApplicationUser { get; set; }
}
