using System.ComponentModel.DataAnnotations.Schema;

namespace Accessories_Store.ViewModels
{
	public class CheckOutVM
	{
		public string Name { get; set; }
		public string Address { get; set; }
		public string Phone { get; set; }
		public string Notes { get; set; }

	}
}

