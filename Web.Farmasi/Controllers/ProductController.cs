using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Web.Farmasi.Models;
using Web.Farmasi.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;


namespace Web.Farmasi.Controllers
{
    public class ProductController : Controller
    {
        private readonly IMongoCollection<Product> _productCollection;
 
        public ProductController(IDbSetting dbSetting, IMongoDatabase mongoDatabase)
        {       
            _productCollection = mongoDatabase.GetCollection<Product>(dbSetting.ProductCollectionName);
        }
        public async Task<IActionResult> Index()
        {
            var products = await _productCollection.Find(x => true).ToListAsync();

            return View(products);
        }


        public IActionResult Product()
        {
            return View();
        }
        public async Task<IActionResult> CreateProduct(Product product)
        {
            await _productCollection.InsertOneAsync(product);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteProduct(string id)
        {
           var result =  await _productCollection.DeleteOneAsync(x=> x.Id==id);
            return Ok(true);
        }

    }
}
