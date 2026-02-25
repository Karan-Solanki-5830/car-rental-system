using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CarRentalFrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            ViewData["PageTitle"] = "Dashboard";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardData()
        {
            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);

            // Fetch data with individual error handling
            async Task<string> SafeGetString(string url)
            {
                try { 
                    var response = await client.GetAsync(url);
                    return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : "[]";
                } catch { return "[]"; }
            }

            async Task<int> SafeGetCount(string url)
            {
                try {
                    var response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode) return 0;
                    var json = await response.Content.ReadAsStringAsync();
                    var obj = JObject.Parse(json);
                    return obj["count"]?.Value<int>() ?? 0;
                } catch { return 0; }
            }

            // Standardize endpoints to match backend controller names (PascalCase often preferred or case-insensitive)
            var userCountTask = SafeGetCount("User/count");
            var customerCountTask = SafeGetCount("Customer/count");
            var vehicleTask = SafeGetString("Vehicle");
            var bookingTask = SafeGetString("Booking");

            await Task.WhenAll(userCountTask, customerCountTask, vehicleTask, bookingTask);

            // Deserialize with fallback to empty lists
            var userCount = await userCountTask;
            var customerCount = await customerCountTask;
            
            var vehicleJson = await vehicleTask;
            var vehicles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.VehicleModel>>(vehicleJson) ?? new List<Models.VehicleModel>();
            
            var bookingJson = await bookingTask;
            var bookings = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.BookingModel>>(bookingJson) ?? new List<Models.BookingModel>();

            // --- Process Data ---
            var vehicleCount = vehicles.Count;
            var bookingCount = bookings.Count;

            // 1. Vehicle Availability
            var availableVehicles = vehicles.Count(v => "Available".Equals(v.Status, StringComparison.OrdinalIgnoreCase));
            var unavailableVehicles = vehicleCount - availableVehicles;

            // 2. Monthly Revenue (Safe calculation)
            var monthlyRevenue = bookings
                .Join(vehicles, b => b.VehicleId, v => v.VehicleId, (b, v) => new
                {
                    Month = b.StartDateTime.ToString("yyyy-MM"),
                    Revenue = (decimal)Math.Max(0.5, (b.EndDateTime - b.StartDateTime).TotalDays) * v.PricePerDay
                })
                .GroupBy(r => r.Month)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Sum(r => r.Revenue));

            // 3. Popular Vehicles
            var popularVehicles = bookings
                .GroupBy(b => b.VehicleId)
                .Select(g => new { VehicleId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .Join(vehicles, pop => pop.VehicleId, v => v.VehicleId, (pop, v) => new { Vehicle = v, Count = pop.Count })
                .ToList();

            // 4. Recent Bookings
            var recentBookings = bookings.OrderByDescending(b => b.StartDateTime).Take(5).ToList();

            return Json(new
            {
                userCount,
                customerCount,
                vehicleCount,
                bookingCount,
                availableVehicles,
                unavailableVehicles,
                monthlyRevenue,
                popularVehicles,
                recentBookings
            });
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

        [AllowAnonymous]
        public IActionResult Unauthorized()
        {
            ViewData["Title"] = "Unauthorized";
            return View();
        }
    }
}
