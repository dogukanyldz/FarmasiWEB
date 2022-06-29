using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Farmasi.Models;
using StackExchange.Redis;
using Web.Farmasi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Newtonsoft.Json;

namespace Web.Farmasi.Controllers
{
    public class BasketController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string userId;
        private readonly IMongoCollection<Product> _productCollection;
        public BasketController(IDbSetting dbSetting,IMongoDatabase mongoDatabase, RedisService redisService , IHttpContextAccessor httpContextAccessor = null)
        {
   
            _productCollection = mongoDatabase.GetCollection<Product>(dbSetting.ProductCollectionName);
            _redisService = redisService;
            _httpContextAccessor = httpContextAccessor;
            userId= _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        public async Task<IActionResult> Index()
        {
       
            var result = await _redisService.GetDb().StringGetAsync(userId);
            if(result.IsNull)
                return View(new BasketViewModel());

            var product = JsonConvert.DeserializeObject<BasketViewModel>(result);
            return View(product);
        }

        public async Task<IActionResult> AddBasketItem(string id)
        {
            var product =  _productCollection.Find(x => x.Id == id).FirstOrDefault();

            var basketItem = new BasketItem { ProductId = product.Id, Price = product.Price, ProductName = product.ProductName };
            var result = await _redisService.GetDb().StringGetAsync(userId);

            var basket = new BasketViewModel();
            if (result.IsNull)          
                basket.basketItems.Add(basketItem);
     
            else
            {
                basket = JsonConvert.DeserializeObject<BasketViewModel>(result);
                if (!basket.basketItems.Any(x=> x.ProductId == id))               
                    basket.basketItems.Add(basketItem);
      
            }
            await _redisService.GetDb().StringSetAsync(userId, JsonConvert.SerializeObject(basket));

            return NoContent();
        }

        public async Task<IActionResult> DeleteById(string id)
        {
            var result = await _redisService.GetDb().StringGetAsync(userId);
            var basket = JsonConvert.DeserializeObject<BasketViewModel>(result);

            if (basket == null)
                return NotFound();

            var deletedItem = basket.basketItems.Where(x => x.ProductId == id).FirstOrDefault();
            if (deletedItem == null)
                return NotFound();

            basket.basketItems.Remove(deletedItem);
            
            await _redisService.GetDb().StringSetAsync(userId, JsonConvert.SerializeObject(basket));

            return RedirectToAction("Index");
        }
    }
}
