using System.Text;
using CarRentalFrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarRentalFrontEnd.Controllers
{
    public class FuelTypeController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FuelTypeController(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Fuel Type Management";
            ViewData["PageTitle"] = "Fuel Type List";
            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var response = await client.GetAsync("fueltype");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var fuelTypes = JsonConvert.DeserializeObject<List<FuelTypeModel>>(json);
                return View(fuelTypes);
            }

            TempData["ErrorMessage"] = "Failed to load fuel types.";
            return View(new List<FuelTypeModel>());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> AddEdit(int? id)
        {
            ViewData["Title"] = "Fuel Type Management";
            ViewData["PageTitle"] = id == null || id == 0 ? "Add Fuel Type" : "Edit Fuel Type";
            await LoadUsersDropdown();

            if (id == null || id == 0)
                return View(new FuelTypeModel());

            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var response = await client.GetAsync($"fueltype/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var fuelType = JsonConvert.DeserializeObject<FuelTypeModel>(json);
                return View(fuelType);
            }

            TempData["ErrorMessage"] = "Failed to load fuel type.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> AddEdit(FuelTypeModel model)
        {
            await LoadUsersDropdown();

            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            if (model.FuelTypeId == 0)
                response = await client.PostAsync("fueltype", content);
            else
                response = await client.PutAsync($"fueltype/{model.FuelTypeId}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = model.FuelTypeId == 0
                    ? "Fuel type added successfully!"
                    : "Fuel type updated successfully!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Failed to save fuel type.";
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)

        {
            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var response = await client.DeleteAsync($"fueltype/{id}");

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Fuel type deleted successfully!";
            else
                TempData["ErrorMessage"] = "Failed to delete fuel type.";

            return RedirectToAction("Index");
        }

        private async Task LoadUsersDropdown()
        {
            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var response = await client.GetAsync("user");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<UserDropdownModel>>(json);
                ViewBag.Users = users;
            }
            else
            {
                ViewBag.Users = new List<UserDropdownModel>();
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
