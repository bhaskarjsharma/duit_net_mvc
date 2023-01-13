using duit_net_mvc.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace duit_net_mvc.Controllers
{
    public class UserAccountController : Controller
    {
        private readonly DatabaseContext db;
        public UserAccountController(DatabaseContext context)
        {
            db = context;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register(string name, string email, string password, string contact)
        {
            User newUser = new User(); // creating object of class User with name newUser

            newUser.Email = email;  // calling setter function of parameter Email
            newUser.Password = HashPassword(password); ;
            newUser.Name = name;
            newUser.ContactNumber = contact;

            db.User.Add(newUser);
            db.SaveChanges();

            TempData["succ_msg"] = "Account has been successfully registerd. Please Login to continue";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string email, string password)
        {

            User user = db.User.Where(m => m.Email == email).First();
            if (user != null)
            {
                if (ValidatePassword(password, user.Password))
                {
                    // login successful
                    var claims = new List<Claim>
                    {
                        new Claim("userid", user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.MobilePhone, user.ContactNumber),
                    };


                    if (user.Name == "AdminUser")
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    }


                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        IssuedUtc = DateTimeOffset.UtcNow,
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // login unsuccessful....gvh
                    return RedirectToAction("Account", "Index");
                }
            }
            else
            {
                // login unsuccessful....gvh
                return RedirectToAction("Account", "Index");
            }
        }

        [HttpGet]
        public IActionResult Forbidden()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            // HttpContext.Session.Clear();
            // Clear the existing external cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        private static string GetRandomSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt(12);
        }
        public static string HashPassword(string password)
        {
            // write manual codes or use existing security library
            return BCrypt.Net.BCrypt.HashPassword(password, GetRandomSalt());
        }
        public static bool ValidatePassword(string password, string correctHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, correctHash);
        }
    }
}
