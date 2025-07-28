using Apiclient_ef.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SystemJson = System.Text.Json;

namespace Apiclient_ef.Controllers
{
	public class TestController : Controller
	{
		private HttpClient CreateHttpClient()
		{
			var client = new HttpClient
			{
				BaseAddress = new Uri("http://localhost:5233/"),
				Timeout = TimeSpan.FromMinutes(3)
			};
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			return client;
		}

		public IActionResult Index() => View();

		public IActionResult Create() => View();

		[HttpPost]
		public async Task<IActionResult> Create(Test test)
		{
			try
			{
				using var client = CreateHttpClient();
				var content = new StringContent(SystemJson.JsonSerializer.Serialize(test), Encoding.UTF8, "application/json");
				var response = await client.PostAsync("api/test/create", content);

				if (!response.IsSuccessStatusCode)
				{
					ViewBag.Message = "Insert failed.";
					return View(test);
				}

				return RedirectToAction("ShowProducts");
			}
			catch (TaskCanceledException)
			{
				ViewBag.Message = "Request timed out.";
				return View(test);
			}
			catch (Exception)
			{
				ViewBag.Message = "Unexpected error occurred.";
				return View(test);
			}
		}

		[HttpGet("Listjson")]
		public async Task<IActionResult> ShowProducts()
		{
			try
			{
				using var client = CreateHttpClient();
				var response = await client.GetAsync("api/test/select");

				if (!response.IsSuccessStatusCode)
					return Json(new { message = "Failed to fetch data" });

				var responseData = await response.Content.ReadAsStringAsync();
				var items = SystemJson.JsonSerializer.Deserialize<List<Test>>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

				return Json(items);
			}
			catch (Exception)
			{
				return Json(new { message = "Unexpected error occurred" });
			}
		}

		[HttpGet("Listtable")]
		public async Task<IActionResult> Get()
		{
			try
			{
				using var client = CreateHttpClient();
				var response = await client.GetAsync("api/test/select");

				if (!response.IsSuccessStatusCode)
				{
					ViewBag.Message = "Failed to fetch data";
					return View(new List<Test>());
				}

				var responseData = await response.Content.ReadAsStringAsync();
				var items = SystemJson.JsonSerializer.Deserialize<List<Test>>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

				return View(items);
			}
			catch (Exception)
			{
				ViewBag.Message = "Unexpected error occurred.";
				return View(new List<Test>());
			}
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			try
			{
				using var client = CreateHttpClient();
				var response = await client.GetAsync($"api/test/{id}");

				if (!response.IsSuccessStatusCode)
					return NotFound();

				var json = await response.Content.ReadAsStringAsync();
				var item = SystemJson.JsonSerializer.Deserialize<Test>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

				return View(item);
			}
			catch (Exception)
			{
				return NotFound();
			}
		}

		[HttpPut("Update")]
		public async Task<IActionResult> Update(int id, Test updated)
		{
			try
			{
				using var client = CreateHttpClient();
				var content = new StringContent(SystemJson.JsonSerializer.Serialize(updated), Encoding.UTF8, "application/json");
				var response = await client.PutAsync($"api/test/update/{updated.Id}", content);

				if (!response.IsSuccessStatusCode)
				{
					ViewBag.Message = "Update failed.";
					return View("Edit", updated);
				}

				return RedirectToAction("ShowProducts");
			}
			catch (Exception)
			{
				ViewBag.Message = "Unexpected error during update.";
				return View("Edit", updated);
			}
		}
	}
}