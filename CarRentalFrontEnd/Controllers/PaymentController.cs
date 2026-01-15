using System.Text;
using CarRentalFrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace CarRentalFrontEnd.Controllers
{
    [Authorize(Roles = "Admin")]

    public class PaymentController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentController(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Payment Management";
            ViewData["PageTitle"] = "Payment List";
            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var response = await client.GetAsync("Payment");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var payments = JsonConvert.DeserializeObject<List<PaymentModel>>(json);
                return View(payments);
            }

            TempData["ErrorMessage"] = "Failed to load payments.";
            return View(new List<PaymentModel>());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddEdit(int? id)
        {
            ViewData["Title"] = "Payment Management";
            ViewData["PageTitle"] = id == null || id == 0 ? "Add Payment" : "Edit Payment";
            await LoadDropdowns();
            var model = new PaymentModel();

            if (id != null && id > 0)
            {
                var client = _clientFactory.CreateClient("CarRentalAPI");
                AttachJwtToken(client);
                var response = await client.GetAsync($"Payment/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<PaymentModel>(json);
                }
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddEdit(PaymentModel payment)
        {
            await LoadDropdowns();

            if (!ModelState.IsValid)
                return View(payment);

            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var json = JsonConvert.SerializeObject(payment);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            if (payment.PaymentId == 0)
                response = await client.PostAsync("Payment", content);
            else
                response = await client.PutAsync($"Payment/{payment.PaymentId}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Payment saved successfully.";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Failed to save payment.";
            return View(payment);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var response = await client.DeleteAsync($"Payment/{id}");

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Payment deleted successfully.";
            else
                TempData["ErrorMessage"] = "Failed to delete payment.";

            return RedirectToAction("Index");
        }

        private async Task LoadDropdowns()
        {
            var client = _clientFactory.CreateClient("CarRentalAPI");
            AttachJwtToken(client);
            var bookingTask = client.GetAsync("Booking/dropdown");
            var userTask = client.GetAsync("User");

            await Task.WhenAll(bookingTask, userTask);

            // Bookings
            if (bookingTask.Result.IsSuccessStatusCode)
            {
                var json = await bookingTask.Result.Content.ReadAsStringAsync();
                var bookings = JsonConvert.DeserializeObject<List<BookingDropdownModel>>(json);
                ViewBag.Bookings = new SelectList(bookings, "bookingId", "display");
            }
            else
            {
                ViewBag.Bookings = new SelectList(new List<object>(), "bookingId", "display");
            }

            // Users
            if (userTask.Result.IsSuccessStatusCode)
            {
                var json = await userTask.Result.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<UserDropdownModel>>(json);
                ViewBag.Users = new SelectList(users, "UserId", "Name");
            }
            else
            {
                ViewBag.Users = new SelectList(new List<object>(), "UserId", "Name");
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
