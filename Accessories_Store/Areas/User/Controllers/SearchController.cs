using Accessories_Store.Areas.Admin.Services;
using Accessories_Store.Data_Access;
using Accessories_Store.Models.Entities;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace Accessories_Store.Areas.User.Controllers
{
    public class SearchController : Controller
    {
        private readonly IProductRepo _productContext;
        private readonly ICategoryRepo _categoryContext;
        private readonly ApplicationDbContext _context;

        public INotyfService _notifyService { get; }
        public SearchController(ApplicationDbContext context, INotyfService notyfService, IProductRepo productContext, ICategoryRepo categoryContext)
        {
            _context = context;
            _notifyService = notyfService;
            _productContext = productContext;
            _categoryContext = categoryContext;
        }
        [HttpGet]
        [Route("/search-by-name")]
        public IActionResult SearchByName(string term)
        {

            var products = _context.Products.Where(x=>x.Published == true && x.Status== 1).Select(x => new
            {
                Id = x.Id,
                Alias = x.Alias,
                Name = x.Name,
                Thumb = x.Thumb,

            }).Where(x=>x.Name.Contains(term)).ToList().Take(7);

            return Json(products);
        }
    }
}
