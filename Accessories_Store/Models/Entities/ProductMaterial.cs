using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Accessories_Store.Models.Entities
{
    public partial class ProductMaterial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }


        public int? Status { get; set; }
        public string? Thumb { get; set; }

        public string? Alias { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
		public virtual ICollection<Product> Products { get; set; } = new List<Product>();

	}

}
