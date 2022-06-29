using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Farmasi.Models
{
    public class BasketItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }

        public decimal Price { get; set; }
    }
}
