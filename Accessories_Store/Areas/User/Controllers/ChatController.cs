using Microsoft.AspNetCore.Mvc;

namespace Accessories_Store.Areas.User.Controllers
{
	[Area("User")]
	public class ChatController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
