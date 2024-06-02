using Microsoft.AspNetCore.Mvc;

namespace Accessories_Store.Controllers
{
    
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
