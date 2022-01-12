using System.Collections.Generic;
using Zenzinger_Eshop_Web.Models.Entity;

namespace Zenzinger_Eshop_Web.Models.Database
{
    public static class eshopDbContext
    {
        public static List<CarouselItem> CarouselItems { get; set; }

        static eshopDbContext()
        {
            DatabaseInit dbInit = new DatabaseInit();
            CarouselItems = dbInit.GenerateCarouselItems();
        }
    }
}