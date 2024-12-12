using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyVideostore.Data;
using MyVideostore.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MyVideostore.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Login Page
        public IActionResult LoginPage()
        {
            // Pass an empty UserAccounts object to the view
            return View(new UserAccounts());
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserAccounts model)
        {
            if (ModelState.IsValid)
            {
                // Check for the user in the database
                var user = await _context.UserAccounts
                                         .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password_ == model.Password_);

                if (user != null)
                {
                    // If login is successful, redirect to a success page
                    return RedirectToAction("Success");
                }

                // If login fails, add an error to the model state
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }

            // Return to the login page with the entered model
            return View("LoginPage", model);
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
