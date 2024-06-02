using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Accessories_Store.Models.Entities;

public partial class OrderDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

	[ForeignKey("Order")]
	public long? OrderId { get; set; }

    [ForeignKey("Product")]
    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public double? Price { get; set; }
	public int? ProductSize { get; set; }


	public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
