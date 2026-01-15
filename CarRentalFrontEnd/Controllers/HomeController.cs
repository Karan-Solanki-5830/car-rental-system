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

            // Fetch all required data in parallel
            var userCountTask = client.GetStringAsync("user/count");
            var customerCountTask = client.GetStringAsync("customer/count");
            var vehicleTask = client.GetStringAsync("vehicle");
            var bookingTask = client.GetStringAsync("booking");

            await Task.WhenAll(userCountTask, customerCountTask, vehicleTask, bookingTask);

            // Deserialize results
            var userCount = JObject.Parse(await userCountTask)["count"]?.Value<int>() ?? 0;
            var customerCount = JObject.Parse(await customerCountTask)["count"]?.Value<int>() ?? 0;
            var vehicles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.VehicleModel>>(await vehicleTask) ?? new List<Models.VehicleModel>();
            var bookings = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.BookingModel>>(await bookingTask) ?? new List<Models.BookingModel>();

            // --- Process Data for Charts and Stats ---

            // 1. Key Performance Indicators (KPIs)
            var userCountValue = userCount;
            var customerCountValue = customerCount;
            var vehicleCount = vehicles.Count;
            var bookingCount = bookings.Count;

            // 2. Vehicle Availability
            var availableVehicles = vehicles.Count(v => "Available".Equals(v.Status, StringComparison.OrdinalIgnoreCase));
            var unavailableVehicles = vehicleCount - availableVehicles;

            // 3. Monthly Revenue
            var monthlyRevenue = bookings
                .Join(vehicles, b => b.VehicleId, v => v.VehicleId, (b, v) => new
                {
                    Month = b.StartDateTime.ToString("yyyy-MM"),
                    Revenue = (decimal)(b.EndDateTime - b.StartDateTime).TotalDays * v.PricePerDay
                })
                .GroupBy(r => r.Month)
                .ToDictionary(g => g.Key, g => g.Sum(r => r.Revenue))
                .OrderBy(kvp => kvp.Key)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // 4. Popular Vehicles (Top 5)
            var popularVehicles = bookings
                .GroupBy(b => b.VehicleId)
                .Select(g => new { VehicleId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .Join(vehicles, pop => pop.VehicleId, v => v.VehicleId, (pop, v) => new { Vehicle = v, pop.Count })
                .ToList();

            // 5. Recent Bookings (for table)
            var recentBookings = bookings.OrderByDescending(b => b.StartDateTime).Take(5).ToList();

            return Json(new
            {
                userCount = userCountValue,
                customerCount = customerCountValue,
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
