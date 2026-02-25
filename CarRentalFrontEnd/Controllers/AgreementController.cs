using System.Text;
using CarRentalFrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CarRentalFrontEnd.Controllers
{
    [Authorize(Roles = "Admin")]

    public class AgreementController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AgreementController(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Agreement Management";
            ViewData["PageTitle"] = "Agreement List";
            try
            {
                var client = _clientFactory.CreateClient("CarRentalAPI");
                AttachJwtToken(client);
                var response = await client.GetAsync("Agreement");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var agreements = JsonConvert.DeserializeObject<List<AgreementModel>>(data);
                    return View(agreements);
                }

                TempData["ErrorMessage"] = "Failed to load agreements.";
                return View(new List<AgreementModel>());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to load agreements. {ex.Message}";
                return View(new List<AgreementModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            ViewData["Title"] = "Agreement Management";
            ViewData["PageTitle"] = id == null || id == 0 ? "Add Agreement" : "Edit Agreement";
            await LoadCustomers();
            var model = new AgreementModel();

            if (id != null && id > 0)
            {
                try
                {
                    var client = _clientFactory.CreateClient("CarRentalAPI");
                    AttachJwtToken(client);
                    var response = await client.GetAsync($"Agreement/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();

                        model = JsonConvert.DeserializeObject<AgreementModel>(json) ?? new AgreementModel();
                        await LoadBookings(model.CustomerId);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to load agreement details.";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Failed to load agreement details. {ex.Message}";
                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(AgreementModel agreement)
        {
            await LoadCustomers();
            await LoadBookings(agreement.CustomerId);

            if (!ModelState.IsValid)
                return View(agreement);

            try
            {
                var client = _clientFactory.CreateClient("CarRentalAPI");
                AttachJwtToken(client);
                var content = new StringContent(JsonConvert.SerializeObject(agreement), Encoding.UTF8, "application/json");

                HttpResponseMessage response;

                if (agreement.AgreementId == 0)
                    response = await client.PostAsync("Agreement", content);
                else
                    response = await client.PutAsync($"Agreement/{agreement.AgreementId}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = agreement.AgreementId == 0
                        ? "Agreement created successfully!"
                        : "Agreement updated successfully!";
                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = "Failed to save agreement.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to save agreement. {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var client = _clientFactory.CreateClient("CarRentalAPI");
                AttachJwtToken(client);
                var response = await client.DeleteAsync($"Agreement/{id}");

                if (response.IsSuccessStatusCode)
                    TempData["SuccessMessage"] = "Agreement deleted.";
                else
                    TempData["ErrorMessage"] = "Failed to delete agreement.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to delete agreement. {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        private async Task LoadCustomers()
        {
            try
            {
                var client = _clientFactory.CreateClient("CarRentalAPI");
                AttachJwtToken(client);
                var response = await client.GetAsync("Customer");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    
                    try 
                    {
                        var apiResult = JsonConvert.DeserializeObject<JObject>(data);
                        if (apiResult != null && apiResult["customers"] != null)
                        {
                            var customers = apiResult["customers"].ToObject<List<CustomerModel>>();
                            ViewBag.Customers = new SelectList(customers ?? new List<CustomerModel>(), "CustomerId", "FullName");
                            return;
                        }
                    }
                    catch (JsonException) { /* Fallback */ }

                    var simpleList = JsonConvert.DeserializeObject<List<CustomerModel>>(data);
                    ViewBag.Customers = new SelectList(simpleList ?? new List<CustomerModel>(), "CustomerId", "FullName");
                }
                else
                {
                    ViewBag.Customers = new SelectList(new List<object>(), "CustomerId", "FullName");
                }
            }
            catch
            {
                ViewBag.Customers = new SelectList(new List<object>(), "CustomerId", "FullName");
            }
        }

        private async Task LoadBookings(int customerId)
        {
            try
            {
                var client = _clientFactory.CreateClient("CarRentalAPI");
                AttachJwtToken(client);
                var response = await client.GetAsync($"Agreement/by-customer/{customerId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var bookings = JsonConvert.DeserializeObject<List<BookingDropdownModel>>(json);
                    var selectList = bookings.Select(b => new SelectListItem { Value = b.bookingId.ToString(), Text = b.display }).ToList();
                    ViewBag.Bookings = selectList;
                }
                else
                {
                    ViewBag.Bookings = new List<SelectListItem>();
                }
            }
            catch
            {
                ViewBag.Bookings = new List<SelectListItem>();
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetBookingsByCustomer(int customerId)
        {
            try
            {
                var client = _clientFactory.CreateClient("CarRentalAPI");
                AttachJwtToken(client);
                var response = await client.GetAsync($"Agreement/by-customer/{customerId}");

                if (!response.IsSuccessStatusCode)
                    return Json(new List<object>());

                var json = await response.Content.ReadAsStringAsync();
                var bookings = JsonConvert.DeserializeObject<List<BookingDropdownModel>>(json);
                return Json(bookings);
            }
            catch
            {
                return Json(new List<object>());
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
