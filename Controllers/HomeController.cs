using duit_net_mvc.Models;
using duit_net_mvc.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace duit_net_mvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly DatabaseContext db;  // Database Context
        private readonly IWebHostEnvironment webHostEnvironment;  // to get server absolute paths
        public HomeController(DatabaseContext context, IWebHostEnvironment webHostEnvironment)
        {
            db = context;
            this.webHostEnvironment = webHostEnvironment;
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
        public async Task<ActionResult> Apply(int advertisementID, string userName, List<IFormFile> fileUploads)
        {
            Advertisement advertisement = db.Advertisement.Find(advertisementID);

            if (advertisement != null)
            {
                Application newApp = new()
                {
                    UserName = userName,
                };

                newApp.User = "user"; // info from current logged in user

                string contentRootPath = webHostEnvironment.ContentRootPath; // will return path for the Content folder
                string[] paths = { contentRootPath, "AppAttachments", advertisement.AdvertisementId.ToString() };
                string fileSaveDir = Path.Combine(paths);
                Directory.CreateDirectory(fileSaveDir);


                List<ApplicationAttachment> attachments = new();
                foreach (var item in fileUploads)
                {
                    if (item.ContentType.Contains("image/") || item.ContentType == "application/pdf")
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                        string fileSavePath = Path.Combine(fileSaveDir, fileName);

                        ApplicationAttachment attachment = new()
                        {
                            FileName = fileName,
                            FileType = item.ContentType,
                            FilePath = fileSavePath
                        };
                        attachments.Add(attachment);

                        // saving large file as stream
                        using var stream = System.IO.File.Create(fileSavePath);
                        await item.CopyToAsync(stream);
                    }
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

        public IActionResult Privacy()
        {
            return View();
        }
    }
}