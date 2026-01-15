using CarRentalFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
                    dynamic apiResult = JsonConvert.DeserializeObject<dynamic>(data);
                    var customers = JsonConvert.DeserializeObject<List<CustomerModel>>(apiResult.customers.ToString());
                    ViewBag.TotalPages = (int)Math.Ceiling((double)apiResult.totalRecords / (int)apiResult.pageSize);
                    ViewBag.CurrentPage = (int)apiResult.page;
                    return View(customers);
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
