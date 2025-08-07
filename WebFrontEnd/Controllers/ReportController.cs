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
    
    public class ReportController : BaseController
    {
        private readonly HttpClient _httpClient;

        public ReportController()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7211/")
            };
        }
        public async Task<IActionResult> DailyReport()
        {
            var response = await _httpClient.GetStringAsync("api/Reporting/GetDailyReport");
            var result = JsonConvert.DeserializeObject<ApiResponseObj>(response);

            if (result.Status)
            {
                var reports = JsonConvert.DeserializeObject<IEnumerable<DailyReport>>(result.Data.ToString());
                return View(reports);
            }
            else
            {
                // Handle error
                return View(new List<DailyReport>());
            }
        }

        public async Task<IActionResult> MonthlyReport()
        {
            var response = await _httpClient.GetStringAsync("api/Reporting/GetMonthlyReport");
            var result = JsonConvert.DeserializeObject<ApiResponseObj>(response);

            if (result.Status)
            {
                var reports = JsonConvert.DeserializeObject<IEnumerable<MonthlyReport>>(result.Data.ToString());
                return View(reports);
            }
            else
            {
                // Handle error
                return View(new List<MonthlyReport>());
            }
        }

        public async Task<IActionResult> YearlyReport()
        {
            var response = await _httpClient.GetStringAsync("api/Reporting/GetYearlyReport");
            var result = JsonConvert.DeserializeObject<ApiResponseObj>(response);

            if (result.Status)
            {
                var reports = JsonConvert.DeserializeObject<IEnumerable<YearlyReport>>(result.Data.ToString());
                return View(reports);
            }
            else
            {
                // Handle error
                return View(new List<YearlyReport>());
            }
        }
    }
}
