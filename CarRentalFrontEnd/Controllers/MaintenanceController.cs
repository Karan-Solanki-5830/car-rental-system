using System.Text;
using CarRentalFrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace CarRentalFrontEnd.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MaintenanceController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MaintenanceController(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Maintenance Management";
            ViewData["PageTitle"] = "Maintenance List";
            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var response = await client.GetAsync("MaintenanceLog");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<MaintenanceModel>>(json);
                return View(list);
            }

            TempData["ErrorMessage"] = "Failed to load maintenance logs.";
            return View(new List<MaintenanceModel>());
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            ViewData["Title"] = "Maintenance Management";
            ViewData["PageTitle"] = id == null || id == 0 ? "Add Maintenance" : "Edit Maintenance";
            await LoadDropdowns();
            var model = new MaintenanceModel();

            if (id != null && id > 0)
            {
                var client = _clientFactory.CreateClient("CarRentalAPI");
                AttachJwtToken(client);
                var response = await client.GetAsync($"MaintenanceLog/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var apiResponse = JsonConvert.DeserializeObject<dynamic>(json);
                        if (apiResponse.data != null)
                            model = JsonConvert.DeserializeObject<MaintenanceModel>(apiResponse.data.ToString());
                        else
                            model = JsonConvert.DeserializeObject<MaintenanceModel>(json);
                    }
                    catch
                    {
                        model = JsonConvert.DeserializeObject<MaintenanceModel>(json);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to load maintenance details.";
                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(MaintenanceModel maintenance)
        {
            await LoadDropdowns();

            if (!ModelState.IsValid)
                return View(maintenance);

            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var content = new StringContent(JsonConvert.SerializeObject(maintenance), Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            if (maintenance.Id == 0)
                response = await client.PostAsync("MaintenanceLog", content);
            else
                response = await client.PutAsync($"MaintenanceLog/{maintenance.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = maintenance.Id == 0
                    ? "Maintenance record created successfully!"
                    : "Maintenance record updated successfully!";
                return RedirectToAction("Index");
            }

            var errorText = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Failed to save maintenance record. Server said: {errorText}";
            await LoadDropdowns();
            return View(maintenance);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var response = await client.DeleteAsync($"MaintenanceLog/{id}");

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Maintenance log deleted.";
            else
                TempData["ErrorMessage"] = "Failed to delete maintenance log.";

            return RedirectToAction("Index");
        }

        private async Task LoadDropdowns()
        {
            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var vehicleTask = client.GetAsync("Vehicle");
            var userTask = client.GetAsync("User");

            await Task.WhenAll(vehicleTask, userTask);

            if (vehicleTask.Result.IsSuccessStatusCode)
            {
                var vehicleJson = await vehicleTask.Result.Content.ReadAsStringAsync();
                var vehicles = JsonConvert.DeserializeObject<List<VehicleModel>>(vehicleJson);
                var vehicleOptions = vehicles.Select(v => new
                {
                    v.VehicleId,
                    Display = $"{v.Brand} - {v.Model}"
                }).ToList();

                ViewBag.Vehicles = new SelectList(vehicleOptions, "VehicleId", "Display");
            }
            else
                ViewBag.Vehicles = new SelectList(new List<object>(), "VehicleId", "Display");

            if (userTask.Result.IsSuccessStatusCode)
            {
                var userJson = await userTask.Result.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<UserModel>>(userJson);
                ViewBag.Users = new SelectList(users, "UserId", "Name");
            }
            else
                ViewBag.Users = new SelectList(new List<object>(), "UserId", "Name");
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
