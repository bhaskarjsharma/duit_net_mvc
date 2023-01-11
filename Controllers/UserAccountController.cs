using duit_net_mvc.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public string Register(string name, string email, string password, string contact)
        {
            User newUser = new User(); // creating object of class User with name newUser

            newUser.Email = email;  // calling setter function of parameter Email
            newUser.Password = password;
            newUser.Name = name;
            newUser.ContactNumber = contact;

            db.User.Add(newUser);
            db.SaveChanges();

            return "";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {

            User user = db.User.Where(m => m.Email == email && m.Password == password).First();
            if (user != null)
            {
                // login successful
                var claims = new List<Claim>
                {
                    new Claim("userid", user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.MobilePhone, user.ContactNumber),
                    new Claim(ClaimTypes.Role, "NormalUser"),
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IssuedUtc = DateTimeOffset.UtcNow,
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Home", "Index");
            }
            else
            {
                // login unsuccessful....gvh
                return RedirectToAction("Account", "Login");
            }
        }

        [HttpGet]
        public IActionResult Forbidden()
        {
            return View();
        }
    }
}
