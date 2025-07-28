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

        // API- LIST DATA FROM SERVER AS JSON DATA
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

        // API- LIST DATA FROM SERVER INTO VIEW PAGE
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

        // API- LOAD THIS DATA INTO EDIT PAGE (TEXTBOXES)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7093/"); // Use your actual base URL

            var response = await client.GetAsync($"api/test/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound(); // or show error message
            }

            var json = await response.Content.ReadAsStringAsync();
            var item = System.Text.Json.JsonSerializer.Deserialize<Test>(json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(item); // Pass the fetched item to Razor view
        }

        // API- UPDATE BY TEXTBOX DATA by the ID
        [HttpPut]
        public async Task<IActionResult> Update(Test updated)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7093/");

            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(updated),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await client.PutAsync($"api/test/update/{updated.Id}", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Message = "Update failed.";
                return View("Edit", updated);
            }

            return RedirectToAction("ShowProducts");
        }

    }
}