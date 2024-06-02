using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Accessories_Store.Models.Entities;

public partial class Rating
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Content { get; set; }

    public int? NumberStars { get; set; }

    [ForeignKey("ApplicationUser")]
    public string? UserId { get; set; }

    [ForeignKey("Product")]
    public int? ProductId { get; set; }
    public int? Status { get; set; }


    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Product? Product { get; set; }
    public ApplicationUser ApplicationUser { get; set; }

}
