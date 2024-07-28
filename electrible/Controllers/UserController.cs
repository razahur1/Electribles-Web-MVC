using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using electrible.Context;
using electrible.Models;
using Microsoft.AspNetCore.Http; // Added for session management
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace electrible.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string SessionKeyName = "LoggedInUser"; // Define session key
        private const string CookieKeyName = "UserCookie"; // Define cookie key

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
                if (existingUser != null)
                {
                    // Username already taken
                    return Json(new { success = false, message = "Username is already taken." });
                }

                // Hash the password
                string hashedPassword = HashPassword(user.Password);
                user.Password = hashedPassword;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (username == "admin" && password == "admin")
            {
                // Handle admin login
                HttpContext.Session.SetString(SessionKeyName, "admin");
                Response.Cookies.Append(CookieKeyName, "admin", new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddMinutes(30) // Set cookie expiration
                });
                return Json(new { success = true, isAdmin = true });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user != null && VerifyPassword(password, user.Password))
            {
                // Handle successful user login
                HttpContext.Session.SetString(SessionKeyName, user.Username); // Store username in session
                Response.Cookies.Append(CookieKeyName, user.Username, new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddMinutes(30) // Set cookie expiration
                });
                return Json(new { success = true, isAdmin = false });
            }

            return Json(new { success = false, message = "Invalid login attempt." });
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(SessionKeyName); // Clear session on logout
            Response.Cookies.Delete(CookieKeyName); // Remove the cookie
            return RedirectToAction("Index", "Home");
        }

        // Method to hash the password
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // Method to verify the password
        private bool VerifyPassword(string password, string hashedPassword)
        {
            string hashedProvidedPassword = HashPassword(password);
            return hashedProvidedPassword == hashedPassword;
        }
    }
}
