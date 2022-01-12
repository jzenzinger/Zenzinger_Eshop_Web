using System.Collections.Generic;
using Zenzinger_Eshop_Web.Models.Entity;

namespace Zenzinger_Eshop_Web.Models.ViewModels
{
    public class IndexViewModel
    {
        public IList<CarouselItem> CarouselItems;
        public IList<Product> Products;
    }
}