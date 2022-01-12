using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Zenzinger_Eshop_Web.Models.Database;
using Zenzinger_Eshop_Web.Models.Entity;
using Zenzinger_Eshop_Web.Models.Implementation;
using Zenzinger_Eshop_Web.Models.ViewModels;

namespace Zenzinger_Eshop_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CarouselController : Controller
    {
        readonly EshopDbContext eshopDbContext;
        private IWebHostEnvironment env;

        public CarouselController(EshopDbContext eshopDb, IWebHostEnvironment env)
        {
            eshopDbContext = eshopDb;
            this.env = env;
        }
        
        // GET
        public IActionResult Select()
        {
            IndexViewModel indexVM = new IndexViewModel();
            indexVM.CarouselItems = eshopDbContext.CarouselItems.ToList();
            
            return View(indexVM);
        }
        // GET
        public IActionResult Create()
        {
            return View();
        }
        // POST
        [HttpPost]
        public async Task<IActionResult> Create(CarouselItem carouselItem)
        {
            // Mozna tohle bude chtit vymazat, kontrola je u ModelState validate action
            /*if(eshopDbContext.CarouselItems != null && eshopDbContext.CarouselItems.Count > 0)
            {
                carouselItem.id = eshopDbContext.CarouselItems.Last().id + 1;
            }*/
                FileUpload fileUpload = new FileUpload(env.WebRootPath, "img/Carousels", "image");
                if (fileUpload.CheckFileContent(carouselItem.Image) &&
                    fileUpload.CheckFileLength(carouselItem.Image))
                {
                    carouselItem.ImageSource = await fileUpload.FileUploadAsync(carouselItem.Image);
                    if (String.IsNullOrWhiteSpace(carouselItem.ImageSource) == false
                        && String.IsNullOrWhiteSpace(carouselItem.ImageAlt) == false)
                    {

                        ModelState.Clear();
                        TryValidateModel(carouselItem);
                        if (ModelState.IsValid)
                        {
                            eshopDbContext.CarouselItems.Add(carouselItem);
                            await eshopDbContext.SaveChangesAsync();

                            return RedirectToAction(nameof(CarouselController.Select));
                        }
                    }
                }
                return View(carouselItem);
        }
        // GET
        public IActionResult Edit(int ID)
        {
            CarouselItem carouselItem = eshopDbContext.CarouselItems.FirstOrDefault(ci => ci.id == ID);
            if (carouselItem != null)
            {
                return View(carouselItem);
            }

            return NotFound();  //Return status 404
        }
        // POST
        [HttpPost]
        public async Task<IActionResult> Edit(CarouselItem cItem)
        {
            CarouselItem carouselItem = eshopDbContext.CarouselItems.FirstOrDefault(ci => ci.id == cItem.id);

            if (carouselItem != null && cItem.Image != null)
            {
                FileUpload fileUpload = new FileUpload(env.WebRootPath, "img/Carousels", "image");
                if (fileUpload.CheckFileContent(cItem.Image) &&
                    fileUpload.CheckFileLength(cItem.Image))
                {
                    cItem.ImageSource = await fileUpload.FileUploadAsync(cItem.Image);
                }
            }

            ModelState.Clear();
            TryValidateModel(cItem);
            if (ModelState.IsValid)
            {
                carouselItem.ImageSource = cItem.ImageSource;
                carouselItem.ImageAlt = cItem.ImageAlt;
                await eshopDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(CarouselController.Select));
            }

            return View(carouselItem);
        }
        // GET
        public async Task<IActionResult> Delete(int ID)
        {
            CarouselItem carouselItem = eshopDbContext.CarouselItems.FirstOrDefault(ci => ci.id == ID);

            if (carouselItem != null)
            {
                eshopDbContext.CarouselItems.Remove(carouselItem);
                await eshopDbContext.SaveChangesAsync();
            }
            
            //Nazev je svazany s danym IAction, ikdyz nam nekdo treba zmeni nazev Action Select
            return RedirectToAction(nameof(CarouselController.Select));       }
    }
}