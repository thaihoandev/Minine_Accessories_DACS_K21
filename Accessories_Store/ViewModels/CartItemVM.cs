namespace Accessories_Store.ViewModels
{
    public class CartItemVM
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Thumb { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
		public double? TotalPrice { get; set; }

		public int? Quantity { get; set; }

        public int? ProductSize { get; set; } 
    }
}
