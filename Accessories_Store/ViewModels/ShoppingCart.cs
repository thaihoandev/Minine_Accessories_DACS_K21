namespace Accessories_Store.ViewModels
{
    public class ShoppingCart
    {
        public List<CartItemVM> Items { get; set; } = new List<CartItemVM>();
        public double? DiscountValue { get; set; }
        public string? CouponCode { get; set; }
        public void AddItem(CartItemVM item)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == item.ProductId && i.ProductSize == item.ProductSize);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }
		public void UpdateItem(CartItemVM item)
		{
			var existingItem = Items.FirstOrDefault(i => i.ProductId == item.ProductId && i.ProductSize == item.ProductSize);

			if (existingItem != null)
			{
				existingItem.Quantity = item.Quantity;
                existingItem.TotalPrice = (item.Price * item.Quantity);
			}
		}
		public void RemoveItem(int id)
        {
            Items.RemoveAll(i => i.Id == id);
        }
		public void ClearCart()
		{
			Items.Clear();
		}
	}
}
