using Accessories_Store.Areas.Admin.Services;
using Accessories_Store.Data_Access;
using Accessories_Store.Models.Entities;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Accessories_Store.Areas.User.Controllers
{
    [Area("User")]
    public class ContactController : Controller
    {
		
		private readonly ApplicationDbContext _context;

		public INotyfService _notifyService { get; }
		public ContactController(ApplicationDbContext context, INotyfService notyfService)
		{
			_context = context;
			_notifyService = notyfService;
		}
		[Route("/contacts")]
		public IActionResult Index()
        {
            return View();
		}


		[HttpPost]
		[Route("contacts/submit-message")]
		public async Task<IActionResult> SubmitMessage([FromBody] Contact contact)
		{
			if (ModelState.IsValid)
			{
				try
				{
					contact.CreatedAt = DateTime.Now;
					_context.Contacts.Add(contact);
					await _context.SaveChangesAsync();
					// Simulate message processing, e.g., save to database or send an email
					await Task.Delay(100); // Simulate an asynchronous operation
					_notifyService.Success("Gửi thành công!");

					// Return a JSON response indicating success
					return Json(new { success = true });
				}
				catch (Exception ex)
				{
					// Log the exception and return a JSON response indicating failure
					// (In a real application, you should log the exception)
					_notifyService.Error("Gửi thất bại!");

					return Json(new { success = false, error = ex.Message });
				}
			}

			// If model state is not valid, return a JSON response indicating failure
			_notifyService.Error("Có lỗi xảy ra!");

			return Json(new { success = false, error = "Invalid form data" });
		}
	}
}
