using CarRentalFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CarRentalFrontEnd.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerController(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Customer
        public async Task<IActionResult> Index(int page = 1)
        {
            ViewData["Title"] = "Customer Management";
            ViewData["PageTitle"] = "Customer List";
            try
            {
                int pageSize = 10;
                var client = _clientFactory.CreateClient("CarRentalAPI");
                AttachJwtToken(client);
                var response = await client.GetAsync($"customer?page={page}&pageSize={pageSize}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    
                    // Try to parse as the expected paged object
                    try 
                    {
                        var apiResult = JsonConvert.DeserializeObject<JObject>(data);
                        if (apiResult != null && apiResult["customers"] != null)
                        {
                            var customers = apiResult["customers"].ToObject<List<CustomerModel>>();
                            ViewBag.TotalPages = (int)Math.Ceiling((double)(apiResult["totalRecords"]?.Value<int>() ?? 0) / (apiResult["pageSize"]?.Value<int>() ?? 10));
                            ViewBag.CurrentPage = apiResult["page"]?.Value<int>() ?? 1;
                            return View(customers ?? new List<CustomerModel>());
                        }
                    }
                    catch (JsonException) { /* Fallback to simple list */ }

                    // Fallback: try to parse as a direct list [{}, {}]
                    var simpleList = JsonConvert.DeserializeObject<List<CustomerModel>>(data);
                    return View(simpleList ?? new List<CustomerModel>());
                }

                TempData["ErrorMessage"] = "Failed to load customers.";
                return View(new List<CustomerModel>());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error fetching customers: {ex.Message}";
                return View(new List<CustomerModel>());
            }
        }


        [HttpGet]

        public async Task<IActionResult> Search(string? name, string? email, string? phone)
        {
            ViewData["Title"] = "Customer Management";
            ViewData["PageTitle"] = "Search Customers";
            try
            {
                var client = _clientFactory.CreateClient("CarRentalAPI");
                AttachJwtToken(client);
                var url = $"customer/search?name={Uri.EscapeDataString(name ?? "")}&email={Uri.EscapeDataString(email ?? "")}&phone={Uri.EscapeDataString(phone ?? "")}";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var customers = JsonConvert.DeserializeObject<List<CustomerModel>>(data);
                    return PartialView("_CustomerTable", customers);
                }

                return PartialView("_CustomerTable", new List<CustomerModel>());
            }
            catch (Exception ex)
            {
                return PartialView("_CustomerTable", new List<CustomerModel>());
            }
        }
        private void AttachJwtToken(HttpClient client)
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
