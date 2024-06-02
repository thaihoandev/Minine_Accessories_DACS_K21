using Microsoft.AspNetCore.Mvc;

namespace Accessories_Store.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
