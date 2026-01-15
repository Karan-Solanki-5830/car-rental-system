using System.Net.Http.Headers;
using System.Text;
using CarRentalFrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarRentalFrontEnd.Controllers
{
    public class VehicleTypeController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VehicleTypeController(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AttachJwtToken(HttpClient client)
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // GET: VehicleType
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Vehicle Type Management";
            ViewData["PageTitle"] = "Vehicle Type List";
            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var response = await client.GetAsync("VehicleType");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var types = JsonConvert.DeserializeObject<List<VehicleTypeModel>>(data);
                return View(types);
            }

            TempData["ErrorMessage"] = "Failed to load vehicle types.";
            return View(new List<VehicleTypeModel>());
        }

        // GET: VehicleType/AddEdit/{id}
        [HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> AddEdit(int? id)
        {
            ViewData["Title"] = "Vehicle Type Management";
            ViewData["PageTitle"] = id == null || id == 0 ? "Add Vehicle Type" : "Edit Vehicle Type";
            var model = new VehicleTypeModel();
            await LoadUsersDropdown();

            if (id != null && id > 0)
            {
                var client = _clientFactory.CreateClient("CarRentalAPI");
                AttachJwtToken(client);
                var response = await client.GetAsync($"vehicletype/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<VehicleTypeModel>(data);
                }
            }

            return View(model);
        }

        // POST: VehicleType/AddEdit
        [HttpPost]
        public async Task<IActionResult> AddEdit(VehicleTypeModel type)
        {
            await LoadUsersDropdown();

            if (!ModelState.IsValid)
                return View(type);

            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var json = JsonConvert.SerializeObject(type);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            if (type.VehicleTypeId == 0)
                response = await client.PostAsync("vehicletype", content);
            else
                response = await client.PutAsync($"vehicletype/{type.VehicleTypeId}", content);

            if (response.IsSuccessStatusCode)
            {
                // Determine id (for create, read from response; for update, use posted id)
                int id = type.VehicleTypeId;
                if (type.VehicleTypeId == 0)
                {
                    var payload = await response.Content.ReadAsStringAsync();
                    try
                    {
                        dynamic obj = JsonConvert.DeserializeObject<dynamic>(payload);
                        id = (int)obj.vehicleTypeId;
                    }
                    catch { }
                }

                TempData["SuccessMessage"] = type.VehicleTypeId == 0
                    ? "Vehicle type added successfully!"
                    : "Vehicle type updated successfully!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Failed to save vehicle type.";
            return View(type);
        }

        private async Task LoadUsersDropdown()
        {
            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var response = await client.GetAsync("user");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<UserDropdownModel>>(data);
                ViewBag.Users = users;
            }
            else
            {
                ViewBag.Users = new List<UserDropdownModel>();
                TempData["ErrorMessage"] = "Failed to load users for dropdown.";
            }
        }// DELETE: VehicleType/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid Vehicle Type ID.";
                return RedirectToAction("Index");
            }

            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var response = await client.DeleteAsync($"vehicletype/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Vehicle Type deleted successfully!";
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Failed to delete Vehicle Type. Status: {response.StatusCode}, {errorMessage}";
            }

            return RedirectToAction("Index");
        }

    }
}
