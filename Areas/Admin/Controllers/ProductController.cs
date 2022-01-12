using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Zenzinger_Eshop_Web.Models.Database;
using Zenzinger_Eshop_Web.Models.Entity;
using Zenzinger_Eshop_Web.Models.Implementation;

namespace Zenzinger_Eshop_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        readonly EshopDbContext eshopDbContext;
        IWebHostEnvironment env;
        public ProductController(EshopDbContext eshopDB, IWebHostEnvironment env)
        {
            eshopDbContext = eshopDB;
            this.env = env;
        }
        public IActionResult Select()
        {
            IList<Product> productItems = eshopDbContext.Products.ToList();
            return View(productItems);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product productItem)
        {
            if (productItem != null && productItem.Image != null)
            {
                FileUpload fileUpload = new FileUpload(env.WebRootPath, "img/ProductItems", "image");
                productItem.ImageSource = await fileUpload.FileUploadAsync(productItem.Image);

                if (String.IsNullOrWhiteSpace(productItem.ImageSource) == false)
                {

                    eshopDbContext.Products.Add(productItem);
                    await eshopDbContext.SaveChangesAsync();

                    return RedirectToAction(nameof(ProductController.Select));

                }
            }
            return View(productItem);

        }

        public IActionResult Edit(int ID)
        {
            Product ciFromDatabase = eshopDbContext.Products.FirstOrDefault(ci => ci.ID == ID);
            if (ciFromDatabase != null)
            {
                return View(ciFromDatabase);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Product productItem)
        {
            Product piFromDatabase = eshopDbContext.Products.FirstOrDefault(pi => pi.ID == productItem.ID);
            if (piFromDatabase != null)
            {
                if (productItem != null && productItem.Image != null)
                {
                    FileUpload fileUpload = new FileUpload(env.WebRootPath, "img/ProductItems", "image");
                    productItem.ImageSource = await fileUpload.FileUploadAsync(productItem.Image);

                    if (String.IsNullOrWhiteSpace(productItem.ImageSource) == false)
                    {


                        piFromDatabase.ImageSource = productItem.ImageSource;
                    }
                }
                piFromDatabase.ImageAlt = productItem.ImageAlt;

                piFromDatabase.Price = productItem.Price;

                piFromDatabase.Info = productItem.Info;

                await eshopDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(ProductController.Select));
            }
            else
            {
                return NotFound();
            }

        }
        public async Task<IActionResult> Delete(int id)
        {
            Microsoft.EntityFrameworkCore.DbSet<Product> productItems = eshopDbContext.Products;

            Product productItem = productItems.FirstOrDefault(productItem => productItem.ID == id);

            if (productItem != null)
            {
                productItems.Remove(productItem);
                await eshopDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ProductController.Select));
        }

    }
}