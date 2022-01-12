using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Zenzinger_Eshop_Web.Models;
using Zenzinger_Eshop_Web.Models.Database;
using Zenzinger_Eshop_Web.Models.ViewModels;

namespace Zenzinger_Eshop_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        readonly EshopDbContext _eshopDbContext;

        public HomeController(ILogger<HomeController> logger, EshopDbContext eshopDb)
        {
            _logger = logger;
            _eshopDbContext = eshopDb;
        }

        public IActionResult Index()
        {
            //_logger
            _logger.LogInformation("Loading Home Index");
            IndexViewModel indexVM = new IndexViewModel();
            indexVM.CarouselItems = _eshopDbContext.CarouselItems.ToList();
            indexVM.Products = _eshopDbContext.Products.ToList();
            
            return View(indexVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
