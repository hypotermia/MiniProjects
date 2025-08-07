using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using WebFrontEnd.Models;

namespace WebFrontEnd.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true // only for dev
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7070/")
            };
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Jika sudah login, langsung ke Index
            if (HttpContext.Session.GetString("IsLoggedIn") == "true")
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseContent);

                if (result != null && result.status == true)
                {
                    // Simpan token & status login di Session
                    HttpContext.Session.SetString("JWToken", (string)result.data);
                    HttpContext.Session.SetString("IsLoggedIn", "true");

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // hapus semua session
            return RedirectToAction("Login", "Home");
        }

        public IActionResult Index()
        {
            // Cek login manual
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
                return RedirectToAction("Login", "Home");

            return View();
        }

        public IActionResult Privacy()
        {
            // Cek login manual
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
                return RedirectToAction("Login", "Home");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
