using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zenzinger_Eshop_Web.Models.Database;

namespace Zenzinger_Eshop_Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly EshopDbContext _dbContext;

        public ProductController(ILogger<ProductController> logger, EshopDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        // GET
        public IActionResult Detail(int id)
        {
            var model = _dbContext.Products.FirstOrDefault(productItem => productItem.ID == id);
            if (model != null)
            {
                return View(model);
            }

            return NotFound();
        }
    }
}