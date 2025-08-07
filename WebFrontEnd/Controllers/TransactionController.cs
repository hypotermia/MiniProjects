using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebFrontEnd.Models;


namespace WebFrontEnd.Controllers
{
    
    public class TransactionController : BaseController
    {
        private readonly HttpClient _httpClient;

        public TransactionController()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7211/")
            };
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetStringAsync("api/Transaction/GetAllTransaction");
            var result = JsonConvert.DeserializeObject<ApiResponseObj>(response);

            if (result.Status)
            {
                var detailTransactions = JsonConvert.DeserializeObject<IEnumerable<DetailTransaction>>(result.Data.ToString());
                return View(detailTransactions);
            }
            else
            {
                // Handle error
                return View(new List<DetailTransaction>());
            }
        }

        public async Task<ActionResult> Create()
        {
            var products = await GetProducts();
            ViewBag.Products = products;
            //var prices = await GetProducts();
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Transaction transaction)
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(transaction), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Transaction/InsertUpdateTransaction", jsonContent);
            var result = JsonConvert.DeserializeObject<ApiResponseObj>(await response.Content.ReadAsStringAsync());

            var responseProd = await _httpClient.GetStringAsync($"https://localhost:7070/api/Products/GetById?uId={transaction.ProductId}");
            var resultProd = JsonConvert.DeserializeObject<ApiResponseObj>(responseProd);
            var product = JsonConvert.DeserializeObject<Product>(resultProd.Data.ToString());

            int remainingQuantity = product.quantity - transaction.Quantity;
            product.quantity = remainingQuantity;

            var jsonContentProduk = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
            var responseProduk = await _httpClient.PostAsync("https://localhost:7070/api/Products/InsertUpdateProducts", jsonContentProduk);
            var resultProduk = JsonConvert.DeserializeObject<ApiResponseObj>(await response.Content.ReadAsStringAsync());


            if (result.Status)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Handle error
                ModelState.AddModelError("", result.Message);
            }
            ViewBag.Title = "Create";
            return View(transaction);
        }

        public async Task<ActionResult> Details(Guid uId)
        {
            var response = await _httpClient.GetAsync($"api/Transaction/GetById?uId={uId}");
            var result = JsonConvert.DeserializeObject<ApiResponseObj>(await response.Content.ReadAsStringAsync());

            if (result.Status)
            {
                var transaction = JsonConvert.DeserializeObject<DetailTransaction>(result.Data.ToString());
                return View(transaction);
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete(Guid uId)
        {
            var response = await _httpClient.PostAsync($"DeleteTransaction?uId={uId}", null);
            var result = JsonConvert.DeserializeObject<ApiResponseObj>(await response.Content.ReadAsStringAsync());

            if (result.Status)
            {
                TempData["Message"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Index");
        }
        public async Task<List<Product>> GetProducts()
        {
            var response = await _httpClient.GetAsync("https://localhost:7070/api/Products/GetAllProducts");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponseObj>(responseContent);

                if (result != null && result.Status)
                {
                    var products = result.Data != null
                        ? JsonConvert.DeserializeObject<List<Product>>(result.Data.ToString())
                        : new List<Product>();
                    return products;
                }
            }
            return new List<Product>();
        }
    }
}
