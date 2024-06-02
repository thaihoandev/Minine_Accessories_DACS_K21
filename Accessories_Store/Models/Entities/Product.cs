using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Accessories_Store.Models.Entities;

public partial class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey("Category")]
    public int? CategoryId { get; set; }

    [Required]
    public string Name { get; set; }

    public string? Description { get; set; }

    public string? Thumb { get; set; }

    public string? Content { get; set; }

    [ForeignKey("ProductMaterial")]
    public int ProductMaterialId { get; set; }

    public int? ProductObjectId { get; set; }
    public int? ProductCollectionId { get; set; }

    public int? Sale { get; set; }

    public bool Newfeed { get; set; }

    public bool Homeflag { get; set; }

    public bool Hot { get; set; }

    public bool Published { get; set; }

    public string? Alias { get; set; }

    public int? Status { get; set; }

    public bool Deleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Category? Category { get; set; }
    public virtual ProductMaterial? ProductMaterial { get; set; }


    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    public virtual ICollection<Keyword> Keywords { get; set; }= new List<Keyword>();


}
