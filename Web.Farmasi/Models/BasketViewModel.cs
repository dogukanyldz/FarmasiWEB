using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Farmasi.Models
{
    public class BasketViewModel
    {
        public BasketViewModel()
        {
            basketItems = new();
        }

        public List<BasketItem> basketItems;

        public decimal TotalPrice => basketItems.Sum(x => x.Price);
    }
}
