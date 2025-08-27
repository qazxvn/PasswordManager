using Microsoft.AspNetCore.Mvc;
using PasswordManeger.Data;
using PasswordManeger.Models;
using PasswordManeger.ViewModels;
using System.Security.Claims;

namespace PasswordManeger.Controllers
{
    public class DashBoardController : Controller
    {
        private readonly AppDbContext _context;

        public DashBoardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult DashBoard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var passwords = _context.UserPasswords
                .Where(p => p.UserId == userId)
                .ToList();

            var model = new DashBoardViewModel
            {
                Passwords = passwords,
                NewPassword = new AddPasswordViewModel()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult AddPassword(AddPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var password = new UserPasswords
                {
                    ServiceName = model.ServiceName,
                    Password = model.Password,
                    Link = model.Link,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };

                _context.UserPasswords.Add(password);
                _context.SaveChanges();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var passwords = _context.UserPasswords
                .Where(p => p.UserId == currentUserId)
                .ToList();

            var vm = new DashBoardViewModel
            {
                Passwords = passwords,
                NewPassword = new AddPasswordViewModel()
            };

            return View("DashBoard", vm);
        }

        public async Task<IActionResult> DeletePassword(int id)
        {
            var passwords = _context.UserPasswords.FirstOrDefault(p => p.Id == id);
            if(passwords != null)
            {
                _context.UserPasswords.Remove(passwords);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("DashBoard"); 
        }
    }
}
