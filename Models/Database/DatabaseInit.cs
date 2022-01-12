using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Zenzinger_Eshop_Web.Models.Entity;
using Zenzinger_Eshop_Web.Models.Entity.Identity;

namespace Zenzinger_Eshop_Web.Models.Database
{
    public class DatabaseInit
    {
        public void Initialization(EshopDbContext eshopDbContext)
        {
            eshopDbContext.Database.EnsureCreated();
            if (eshopDbContext.CarouselItems.Count() == 0)
            {
                IList<CarouselItem> cItems = GenerateCarouselItems();
                foreach (var ci in cItems)
                {
                    eshopDbContext.CarouselItems.Add(ci);
                }
                eshopDbContext.SaveChanges();
            }
            if (eshopDbContext.Products.Count() == 0)
            {
                IList<Product> products = GenerateProducts();
                foreach (var prod in products)
                {
                    eshopDbContext.Products.Add(prod);
                }
                eshopDbContext.SaveChanges();
            }
        }
        public List<CarouselItem> GenerateCarouselItems()
        {
            List<CarouselItem> carouselItems = new List<CarouselItem>();

            CarouselItem ci1 = new CarouselItem()
            {
                id = 0,
                ImageSource = "./img/1-code.jpg",
                ImageAlt = "First slide"
            };
            CarouselItem ci2 = new CarouselItem()
            {
                id = 1,
                ImageSource = "./img/2-code.jpg",
                ImageAlt = "Second slide"
            };
            CarouselItem ci3 = new CarouselItem()
            {
                id = 2,
                ImageSource = "./img/3-code.jpg",
                ImageAlt = "Third slide"
            };
            
            carouselItems.Add(ci1);
            carouselItems.Add(ci2);
            carouselItems.Add(ci3);


            return carouselItems;
        }
        
        public List<Product> GenerateProducts()
        {
            List<Product> products = new List<Product>();

            Product p1 = new Product()
            {
                //ID = 0,
                ImageSource = "./img/Products/p-1.png",
                Info = "iPhone 13 Pro",
                ImageAlt = "iPhone 13 Pro",
                Name = "Just Pro like you.",
                Price = 1199,
            };
            Product p2 = new Product()
            {
                //ID = 1,
                ImageSource = "./img/Products/p-2.jfif",
                Info = "iPhone 13 Pro Blue",
                ImageAlt = "iPhone 13 Pro  Blue",
                Name = "Newest and best iPhone Pro ever",
                Price = 1299,
            };
            Product p3 = new Product()
            {
                //ID = 2,
                ImageSource = "./img/Products/p-3.jfif",
                Info = "iPhone 13 Pro Space Gray",
                ImageAlt = "iPhone 13 Pro Space Gray",
                Name = "Darkest and most elegant iPhone ever",
                Price = 1199,
            };
            Product p4 = new Product()
            {
                //ID = 3,
                ImageSource = "./img/Products/p-5.png",
                Info = "iPhone 13 Saphire Blue",
                ImageAlt = "iPhone 13 Saphire Blue",
                Name = "Best power compare to price",
                Price = 899,
            };
            Product p5 = new Product()
            {
                //ID = 4,
                ImageSource = "./img/Products/p-6.jfif",
                Info = "iPhone 13 Mini Black",
                ImageAlt = "iPhone 13 Mini Black",
                Name = "Smallest and best price",
                Price = 799,
            };

            products.Add(p1);
            products.Add(p2);
            products.Add(p3);
            products.Add(p4);
            products.Add(p5);

            return products;
        }

        public async Task EnsureRoleCreated(RoleManager<Role> roleManager)
        {
            string[] roles = Enum.GetNames(typeof(Roles));

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new Role(role));
            }
        }

        public async Task EnsureAdminCreated(UserManager<User> userManager)
        {
            User user = new User
            {
                UserName = "admin",
                Email = "admin@admin.cz",
                EmailConfirmed = true,
                FirstName = "Jiri",
                LastName = "JiriAdmin"
            };
            string password = "admin";

            User adminInDatabase = await userManager.FindByNameAsync(user.UserName);

            if (adminInDatabase == null)
            {

                IdentityResult result = await userManager.CreateAsync(user, password);

                if (result == IdentityResult.Success)
                {
                    string[] roles = Enum.GetNames(typeof(Roles));
                    foreach (var role in roles)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
                else if (result != null && result.Errors != null && result.Errors.Count() > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        Debug.WriteLine($"Error during Role creation for Admin: {error.Code}, {error.Description}");
                    }
                }
            }

        }

        public async Task EnsureManagerCreated(UserManager<User> userManager)
        {
            User user = new User
            {
                UserName = "manager",
                Email = "manager@manager.cz",
                EmailConfirmed = true,
                FirstName = "Jiri",
                LastName = "JiriManager"
            };
            string password = "manager";

            User managerInDatabase = await userManager.FindByNameAsync(user.UserName);

            if (managerInDatabase == null)
            {

                IdentityResult result = await userManager.CreateAsync(user, password);

                if (result == IdentityResult.Success)
                {
                    string[] roles = Enum.GetNames(typeof(Roles));
                    foreach (var role in roles)
                    {
                        if (role != Roles.Admin.ToString())
                            await userManager.AddToRoleAsync(user, role);
                    }
                }
                else if (result != null && result.Errors != null && result.Errors.Count() > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        Debug.WriteLine($"Error during Role creation for Manager: {error.Code}, {error.Description}");
                    }
                }
            }

        }
    }
}