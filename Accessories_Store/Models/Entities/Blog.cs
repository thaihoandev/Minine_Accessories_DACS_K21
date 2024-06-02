using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Accessories_Store.Models.Entities;

public partial class Blog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Title { get; set; }

    public string? Scontent { get; set; }

    public string? Content { get; set; }

    public string? Thumb { get; set; }

    public bool Published { get; set; }

    public string? Alias { get; set; }

    public string? Author { get; set; }

    [ForeignKey("ApplicationUser")]
    public string? UserId { get; set; }

    public int? Views { get; set; }
    public int? Status { get; set; }


    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

}
