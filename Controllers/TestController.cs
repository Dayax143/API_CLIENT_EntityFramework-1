using Apiclient_ef.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using SystemJson = System.Text.Json;

namespace Apiclient_ef.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet("Listjson")]
        public async Task<IActionResult> ShowProducts()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7093/");

            var response = await client.GetAsync("api/test/select");

            if (!response.IsSuccessStatusCode)
            {
                return Json(new { message = "Failed to fetch data" });
            }

            // ⬇️ Make sure this line is inside the scope where `response` is available
            var responseData = await response.Content.ReadAsStringAsync();

            var items = System.Text.Json.JsonSerializer.Deserialize<List<Test>>(responseData, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Json(items);

        }


        [HttpGet("Listtable")]
        public async Task<IActionResult> Get()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7093/");

            var response = await client.GetAsync("api/test/select");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Message = "Failed to fetch data";
                return View(new List<Test>());
            }

            var responseData = await response.Content.ReadAsStringAsync();

            var items = System.Text.Json.JsonSerializer.Deserialize<List<Test>>(responseData, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(items); // ⬅️ This passes your data to the Razor view
        }

    }
}