using duit_net_mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace duit_net_mvc.Controllers
{
    [Authorize]
    [Authorize(Roles = "Admin")]

    public class AdminController : Controller
    {
        private readonly DatabaseContext db;
        public AdminController(DatabaseContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            if(this.User.IsInRole("Admin")){

            }

            string userid = this.User.FindFirst(x => x.Type == "userid").Value;
            string name = this.User.FindFirst(x => x.Type == ClaimTypes.Name).Value;

            return View();
        }
        [HttpGet]
        public IActionResult AddAdvertisement()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddAdvertisement(string Title, string Description, DateTime StartDate, DateTime EndDate)
        {
            Advertisement newAdv = new()
            {
                Title = Title,
                Description = Description,
                StartDate = StartDate,
                EndDate = EndDate,
            };
            db.Advertisement.Add(newAdv);
            db.SaveChanges();

            return View();
        }

    }
}
