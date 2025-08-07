using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebFrontEnd.Models;

namespace WebFrontEnd.Controllers
{
    
    public class ProductController : BaseController
    {
        private readonly HttpClient _httpClient;

        public ProductController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7070/"); // Replace with your API base URL
        }

        public async Task<IActionResult> Index()
        {
            var isLoggedIn = HttpContext.Session.GetString("IsLoggedIn");
            if (isLoggedIn != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            var response = await _httpClient.GetStringAsync("api/Products/GetAllProducts");
            var result = JsonConvert.DeserializeObject<ApiResponseObj>(response);

            if (result.Status)
            {
                var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(result.Data.ToString());
                return View(products);
            }
            else
            {
                // Handle error
                return View(new List<Product>());
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _httpClient.GetStringAsync($"api/Products/GetById?uId={id}");
            var result = JsonConvert.DeserializeObject<ApiResponseObj>(response);

            if (result.Status)
            {
                var product = JsonConvert.DeserializeObject<Product>(result.Data.ToString());
                return View(product);
            }
            else
            {
                // Handle error
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Create";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                var jsonContent = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Products/InsertUpdateProducts", jsonContent);
                var result = JsonConvert.DeserializeObject<ApiResponseObj>(await response.Content.ReadAsStringAsync());

                if (result.Status)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Handle error
                    ModelState.AddModelError("", result.Message);
                }
            }

            ViewBag.Title = "Create";
            return View(product);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _httpClient.GetStringAsync($"api/Products/GetById?uId={id}");
            var result = JsonConvert.DeserializeObject<ApiResponseObj>(response);

            if (result.Status)
            {
                var product = JsonConvert.DeserializeObject<Product>(result.Data.ToString());
                ViewBag.Title = "Edit";
                return View("Edit", product);
            }
            else
            {
                // Handle error
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                var jsonContent = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Products/InsertUpdateProducts", jsonContent);
                var result = JsonConvert.DeserializeObject<ApiResponseObj>(await response.Content.ReadAsStringAsync());

                if (result.Status)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Handle error
                    ModelState.AddModelError("", result.Message);
                }
            }

            ViewBag.Title = "Edit";
            return View("Create", product);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _httpClient.GetStringAsync($"api/Products/GetById?uId={id}");
            var result = JsonConvert.DeserializeObject<ApiResponseObj>(response);

            if (result.Status)
            {
                var product = JsonConvert.DeserializeObject<Product>(result.Data.ToString());
                return View(product);
            }
            else
            {
                // Handle error
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _httpClient.PostAsync($"api/Products/DeleteProducts?uId={id}", null);
            var result = JsonConvert.DeserializeObject<ApiResponseObj>(await response.Content.ReadAsStringAsync());

            if (result.Status)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Handle error
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

