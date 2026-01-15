using System.Text;
using CarRentalFrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarRentalFrontEnd.MVC.Controllers
{
    public class VehicleController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VehicleController(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Vehicle
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Vehicle Management";
            ViewData["PageTitle"] = "Vehicle List";

            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var response = await client.GetAsync("vehicle");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var vehicles = JsonConvert.DeserializeObject<List<VehicleModel>>(data);
                if (vehicles == null || vehicles.Count == 0)
                {
                    TempData["ErrorMessage"] = "No vehicles found.";
                    return View(new List<VehicleModel>());
                }
                return View(vehicles);
            }

            TempData["ErrorMessage"] = "Failed to load vehicles.";
            return View(new List<VehicleModel>());
        }

        // GET: /Vehicle/Search?brand=&model=&plate=&status=
        [HttpGet]
        public async Task<IActionResult> Search(string? brand, string? model, string? plate, string? status)
        {
            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var response = await client.GetAsync("vehicle");

            var list = new List<VehicleModel>();
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                list = JsonConvert.DeserializeObject<List<VehicleModel>>(data) ?? new List<VehicleModel>();
            }

            IEnumerable<VehicleModel> filtered = list;
            if (!string.IsNullOrWhiteSpace(brand))
                filtered = filtered.Where(v => v.Brand != null && v.Brand.Contains(brand, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(model))
                filtered = filtered.Where(v => v.Model != null && v.Model.Contains(model, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(status))
                filtered = filtered.Where(v => v.Status != null && v.Status.Contains(status, StringComparison.OrdinalIgnoreCase));

            return PartialView("~/Views/Vehicle/_VehicleTable.cshtml", filtered.ToList());
        }

        // GET: Vehicle/AddEdit/{id}
        [HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> AddEdit(int? id)
        {
            ViewData["Title"] = "Vehicle Management";
            ViewData["PageTitle"] = id == null || id == 0 ? "Add Vehicle" : "Edit Vehicle";
            await LoadDropdowns();

            if (id == null || id == 0)
                return View(new VehicleModel());

            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var response = await client.GetAsync($"vehicle/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Unable to load vehicle data.";
                return RedirectToAction("Index");
            }

            var data = await response.Content.ReadAsStringAsync();
            var vehicle = JsonConvert.DeserializeObject<VehicleModel>(data);
            return View(vehicle);
        }

        // POST: Vehicle/AddEdit
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> AddEdit(VehicleModel vehicle)
        {
            await LoadDropdowns();

            if (!ModelState.IsValid)
                return View(vehicle);

            try
            {
                var client = _clientFactory.CreateClient("CarRentalAPI");
                AttachJwtToken(client);

                var jsonData = JsonConvert.SerializeObject(vehicle);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = vehicle.VehicleId == 0
                    ? await client.PostAsync("vehicle", content)
                    : await client.PutAsync($"vehicle/{vehicle.VehicleId}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = vehicle.VehicleId == 0
                        ? "Vehicle added successfully!"
                        : "Vehicle updated successfully!";
                    return RedirectToAction("Index");
                }

                var error = await response.Content.ReadAsStringAsync();
                ViewBag.Error = "Failed to save vehicle: " + error;
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred while saving the vehicle: " + ex.Message;
            }

            return View(vehicle);
        }

        // GET: Vehicle/Delete/{id}
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int id)
        {
            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var response = await client.DeleteAsync($"vehicle/{id}");

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Vehicle deleted successfully!";
            else
                TempData["ErrorMessage"] = "Failed to delete vehicle.";

            return RedirectToAction("Index");
        }

        // Load dropdown data
        [Authorize(Roles = "Admin")]

        private async Task LoadDropdowns()
        {
            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            try
            {
                // Get Users
                var usersResponse = await client.GetAsync("user");
                if (usersResponse.IsSuccessStatusCode)
                {
                    var usersJson = await usersResponse.Content.ReadAsStringAsync();
                    ViewBag.Users = JsonConvert.DeserializeObject<List<UserModel>>(usersJson) ?? new List<UserModel>();
                }
                else
                {
                    Console.WriteLine($"Failed to load users: {usersResponse.StatusCode}");
                    ViewBag.Users = new List<UserModel>();
                }

                // Get Fuel Types
                var fuelTypesResponse = await client.GetAsync("fueltype");
                if (fuelTypesResponse.IsSuccessStatusCode)
                {
                    var fuelJson = await fuelTypesResponse.Content.ReadAsStringAsync();
                    ViewBag.FuelTypes = JsonConvert.DeserializeObject<List<FuelTypeModel>>(fuelJson) ?? new List<FuelTypeModel>();
                }
                else
                {
                    Console.WriteLine($"Failed to load fuel types: {fuelTypesResponse.StatusCode}");
                    ViewBag.FuelTypes = new List<FuelTypeModel>();
                }

                // Get Vehicle Types
                var vehicleTypesResponse = await client.GetAsync("vehicletype");
                if (vehicleTypesResponse.IsSuccessStatusCode)
                {
                    var typeJson = await vehicleTypesResponse.Content.ReadAsStringAsync();
                    var vehicleTypes = JsonConvert.DeserializeObject<List<VehicleTypeModel>>(typeJson) ?? new List<VehicleTypeModel>();
                    ViewBag.VehicleTypes = vehicleTypes;
                    Console.WriteLine($"Loaded {vehicleTypes.Count} vehicle types");
                }
                else
                {
                    var errorContent = await vehicleTypesResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to load vehicle types: {vehicleTypesResponse.StatusCode} - {errorContent}");
                    ViewBag.VehicleTypes = new List<VehicleTypeModel>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoadDropdowns: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                ViewBag.Users = new List<UserModel>();
                ViewBag.FuelTypes = new List<FuelTypeModel>();
                ViewBag.VehicleTypes = new List<VehicleTypeModel>();
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