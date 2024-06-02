using Accessories_Store.Areas.Admin.Services;
using Accessories_Store.Data_Access;
using Accessories_Store.Helpers;
using Accessories_Store.Models;
using Accessories_Store.Models.Entities;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;

namespace Accessories_Store.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class AdminContactController : Controller
	{
		
		private readonly ICategoryRepo _categoryContext;
		private readonly ApplicationDbContext _context;

		public INotyfService _notifyService { get; }
		public AdminContactController(ApplicationDbContext context, INotyfService notyfService, ICategoryRepo categoryContext)
		{
			_context = context;
			_notifyService = notyfService;
			_categoryContext = categoryContext;
		}
		[Route("/admin/contacts")]
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;
            IEnumerable<Contact> lsContact = new List<Contact>();
			lsContact = _context.Contacts.ToList();
            PagedList<Contact> models = new PagedList<Contact>(lsContact.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;


            return View(models);
        }
    }
}
