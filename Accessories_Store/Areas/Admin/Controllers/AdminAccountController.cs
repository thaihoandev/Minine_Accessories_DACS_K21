using Accessories_Store.Areas.Admin.Services;
using Accessories_Store.Data_Access;
using Accessories_Store.Helpers;
using Accessories_Store.Models;
using Accessories_Store.Models.Entities;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace Accessories_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AdminAccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public INotyfService _notifyService { get; }
        public AdminAccountController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, INotyfService notyfService)
        {
            _context = context;
            _notifyService = notyfService;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Route("/admin/accounts/admin")]
        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;

            var adminRole = await _roleManager.FindByNameAsync(SD.Role_Admin);
            var usersInAdminRole = await _userManager.GetUsersInRoleAsync(adminRole.Name);
            var users = usersInAdminRole.AsQueryable();

            PagedList<ApplicationUser> models = new PagedList<ApplicationUser>(users, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;

            return View(models);
        }
        [Route("/admin/accounts/customer")]
        public async Task<IActionResult> Customer(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;

            var adminRole = await _roleManager.FindByNameAsync(SD.Role_Admin);
            var usersInAdminRole = await _userManager.GetUsersInRoleAsync(adminRole.Name);
            var users = _context.ApplicationUsers.Where(user => !usersInAdminRole.Contains(user)).AsQueryable();

            PagedList<ApplicationUser> models = new PagedList<ApplicationUser>(users, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;

            return View(models);
        }

        // GET: Admin/AdminAccount/Delete/5
        [Route("/admin/accounts/delete/{id}")]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || _context.ApplicationUsers == null)
            {
                return NotFound();
            }

            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/AdminAccount/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("/admin/accounts/delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.ApplicationUsers == null)
            {
                return Problem("Entity set 'DbEcommerceMarketContext.ApplicationUsers'  is null.");
            }
            var user = await _context.ApplicationUsers.FindAsync(id);
            if (user != null)
            {
                user.Active = false;
                _context.ApplicationUsers.Update(user);
            }

            await _context.SaveChangesAsync();
            _notifyService.Success("Xóa tài khoản thành công!");
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(string id)
        {
            return (_context.ApplicationUsers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
