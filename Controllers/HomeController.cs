using duit_net_mvc.Models;
using duit_net_mvc.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace duit_net_mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseContext db;
        public HomeController(DatabaseContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CurrentOpenings()
        {
            List<Advertisement> advertisements = db.Advertisement.ToList();
            return View(advertisements);
        }

        public IActionResult UserList()
        {
            List<User> users = db.User.ToList();
            return View(users);
        }
        [HttpGet]
        public IActionResult Apply(int id)
        {
            Advertisement advertisement = db.Advertisement.Find(id);
            if(advertisement != null)
            {
                ViewBag.advt_id = id;
                ViewBag.advt_title = advertisement.Title;
                TempData["data"] = "data";

                ApplicationViewModel newVM = new()
                {
                    AdvertisementId = advertisement.AdvertisementId
                };
                return View(newVM);
            }
            else
            {
                TempData["error_msg"] = "Invalid Advertisement";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult Apply(int advertisementID, string userName, List<IFormFile> fileUploads)
        {
            Advertisement advertisement = db.Advertisement.Find(advertisementID);

            if (advertisement != null)
            {
                Application newApp = new()
                {
                    UserName = userName,
                };

                newApp.User = "user"; // info from current logged in user

                List<ApplicationAttachment> attachments = new();
                foreach (var item in fileUploads)
                {
                    ApplicationAttachment attachment = new()
                    {
                        FileName = item.FileName,
                        FileType = item.ContentType,
                        FilePath = ""
                    };

                    attachments.Add(attachment);
                }

                newApp.ApplicationAttachment = attachments;

                if (advertisement.Application != null && advertisement.Application.Any())
                {
                    advertisement.Application.Add(newApp);
                }
                else
                {
                    List<Application> applications = new();

                    applications.Add(newApp);

                    advertisement.Application = applications;
                }

                db.Entry(advertisement).State = EntityState.Modified;
                db.SaveChanges();
            }

            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}