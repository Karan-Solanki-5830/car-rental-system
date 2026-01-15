using System.Text;
using CarRentalFrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarRentalFrontEnd.Controllers
{
    public class BookingController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BookingController(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Booking
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Booking Management";
            ViewData["PageTitle"] = "Booking List";
            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var response = await client.GetAsync("Booking");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var bookings = JsonConvert.DeserializeObject<List<BookingModel>>(json) ?? new List<BookingModel>();

                // Fetch vehicles to enrich booking list with brand & model
                var vehicleResponse = await client.GetAsync("vehicle");
                if (vehicleResponse.IsSuccessStatusCode)
                {
                    var vehicleJson = await vehicleResponse.Content.ReadAsStringAsync();
                    var vehicles = JsonConvert.DeserializeObject<List<VehicleModel>>(vehicleJson) ?? new List<VehicleModel>();
                    var vehicleMap = vehicles.ToDictionary(v => v.VehicleId, v => v);

                    foreach (var b in bookings)
                    {
                        if (vehicleMap.TryGetValue(b.VehicleId, out var v))
                        {
                            b.VehicleBrand = v.Brand;
                            b.VehicleModel = v.Model;
                        }
                    }
                }
                return View(bookings);
            }

            var errorText = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Failed to load bookings. Server said: {errorText}";
            return View(new List<BookingModel>());
        }

        // GET: Booking/AddEdit/{id}
        [Authorize(Roles = "Admin")]

        [HttpGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            ViewData["Title"] = "Booking Management";
            ViewData["PageTitle"] = id == null || id == 0 ? "Add Booking" : "Edit Booking";
            await LoadDropdowns();
            var model = new BookingModel();

            if (id != null && id > 0)
            {
                var client = _clientFactory.CreateClient("CarRentalAPI");
                AttachJwtToken(client);
                var response = await client.GetAsync($"Booking/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<BookingModel>(json);
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to load booking details.";
                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        // POST: Booking/AddEdit
        [Authorize(Roles = "Admin")]

        [HttpPost]
        public async Task<IActionResult> AddEdit(BookingModel booking)
        {
            await LoadDropdowns();

            if (!ModelState.IsValid)
                return View(booking);

            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var content = new StringContent(JsonConvert.SerializeObject(booking), Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            if (booking.BookingId == 0)
                response = await client.PostAsync("Booking", content);
            else
                response = await client.PutAsync($"Booking/{booking.BookingId}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = booking.BookingId == 0
                    ? "Booking created successfully!"
                    : "Booking updated successfully!";
                return RedirectToAction("Index");
            }

            var error = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Failed to save booking. Server said: {error}";
            return RedirectToAction("Index");

        }


        // GET: Booking/Delete/{id}
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int id)
        {
            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var response = await client.DeleteAsync($"Booking/{id}");

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Booking deleted successfully!";
            else
                TempData["ErrorMessage"] = "Failed to delete booking.";

            return RedirectToAction("Index");
        }

        // Load dropdowns for Customer and Vehicle
        private async Task LoadDropdowns()
        {
            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var customerTask = client.GetAsync("customer");
            var vehicleTask = client.GetAsync("vehicle");

            await Task.WhenAll(customerTask, vehicleTask);

            // Customers
            if (customerTask.Result.IsSuccessStatusCode)
            {
                var customerJson = await customerTask.Result.Content.ReadAsStringAsync();
                try
                {
                    // Try to deserialize as paginated response first
                    var paginatedResponse = JsonConvert.DeserializeObject<dynamic>(customerJson);
                    if (paginatedResponse?.customers != null)
                    {
                        ViewBag.Customers = paginatedResponse.customers.ToObject<List<CustomerModel>>() ?? new List<CustomerModel>();
                    }
                    else
                    {
                        // Fallback to direct list deserialization if not a paginated response
                        ViewBag.Customers = JsonConvert.DeserializeObject<List<CustomerModel>>(customerJson) ?? new List<CustomerModel>();
                    }
                }
                catch
                {
                    // If deserialization fails, set empty list
                    ViewBag.Customers = new List<CustomerModel>();
                }
            }
            else
            {
                ViewBag.Customers = new List<CustomerModel>();
            }

            // Vehicles
            if (vehicleTask.Result.IsSuccessStatusCode)
            {
                var vehicleJson = await vehicleTask.Result.Content.ReadAsStringAsync();
                try
                {
                    var vehicles = JsonConvert.DeserializeObject<List<VehicleModel>>(vehicleJson) ?? new List<VehicleModel>();
                    ViewBag.Vehicles = vehicles;
                }
                catch
                {
                    ViewBag.Vehicles = new List<VehicleModel>();
                }
            }
            else
            {
                ViewBag.Vehicles = new List<VehicleModel>();
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
