using Newtonsoft.Json;
using NUMLPay_WebApp.Models;
using NUMLPay_WebApp.Services;
using NUMLPay_WebApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Controllers
{
    [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 1 })]
    public class BusRouteController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        CampusService campusService;
        usersTypeService usersTypeService;
        statusService StatusService;
        facultyService facultyServices;
        busRouteService busRouteService;
        apiService apiServices;

        public BusRouteController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            campusService = new CampusService(baseAddress);
            usersTypeService = new usersTypeService();
            StatusService = new statusService();
            facultyServices = new facultyService(baseAddress);
            busRouteService = new busRouteService(baseAddress);
            apiServices = new apiService(baseAddress.ToString());
        }

        public async Task<ActionResult> addBusRoute()
        {
            Admin admin = userAccessAdmin();

            ViewBag.Display = "none;";
            return View();
        }

        // Add BusRoute Post
        [HttpPost]
        public async Task<ActionResult> addBusRoute(BusRoute busRoute)
        {
            Admin admin = userAccessAdmin();

            busRoute.campus_id = Convert.ToInt32(admin.campus_id);
            busRoute.added_by = admin.email_id;
            busRoute.is_active = 1;

            HttpResponseMessage responseMessage = await apiServices.PostAsync("BusRoute", busRoute);

            if (responseMessage.IsSuccessStatusCode)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                ViewBag.AlertMessage = responseData;
                ViewBag.AlertType = "alert-success";
                ViewBag.Display = "block;";
            }
            else
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;

                ViewBag.AlertMessage = responseData;
                ViewBag.AlertType = "alert-danger";
                ViewBag.Display = "block;";
            }

            return View();
        }

        // View all BusRoutes
        public async Task<ActionResult> viewBusRoutes()
        {
            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            Admin admin = userAccessAdmin();

            List<BusRouteView> listBusRoutes = await getBusRoutes(Convert.ToInt32(admin.campus_id));

            return View(listBusRoutes);
        }

        // Update BusRoute
        public async Task<ActionResult> updateBusRoute(int Id)
        {
            userAccessAdmin();

            BusRoute busRoute = null;

            using (var response = await apiServices.GetAsync($"BusRoute/Get/{Id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    busRoute = JsonConvert.DeserializeObject<BusRoute>(data);
                }
            }

            ViewBag.Display = "none;";

            int? selectedValue = busRoute.is_active;

            ViewBag.is_active = StatusService.getStatus(selectedValue);

            return View(busRoute);
        }

        // Update BusRoute
        [HttpPost]
        public async Task<ActionResult> updateBusRoute(BusRoute busRoute)
        {
            Admin admin = userAccessAdmin();

            ViewBag.Display = "none;";

            busRoute.added_by = admin.email_id;
            busRoute.campus_id = Convert.ToInt32(admin.campus_id);

            HttpResponseMessage responseMessage = await apiServices.PutAsync($"BusRoute/Update/{busRoute.id}", busRoute);

            if (responseMessage.IsSuccessStatusCode)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                ViewBag.AlertMessage = responseData;
                ViewBag.AlertType = "alert-success";
                ViewBag.Display = "block;";
            }
            else
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                ViewBag.AlertMessage = responseData;
                ViewBag.AlertType = "alert-danger";
                ViewBag.Display = "block;";
            }

            List<BusRouteView> listBusRoutes = await getBusRoutes(Convert.ToInt32(admin.campus_id));

            return View("viewBusRoutes", listBusRoutes);
        }

        // Delete BusRoute
        public async Task<ActionResult> deleteBusRoute(int? Id)
        {
            Admin admin = userAccessAdmin();

            List<BusRouteView> listBusRoutes = await getBusRoutes(Convert.ToInt32(admin.campus_id));

            if (!Id.HasValue)
            {
                return RedirectToAction("viewBusRoutes");
            }
            else
            {
                BusRoute busRoute = new BusRoute();
                busRoute.is_active = 2;

                HttpResponseMessage responseMessage = await apiServices.PutAsync($"BusRoute/Deactivate/{Id}", busRoute);

                if (responseMessage.IsSuccessStatusCode)
                {
                    string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    ViewBag.AlertMessage = responseData;
                    ViewBag.AlertType = "alert-danger";
                    ViewBag.Display = "block;";
                }
                else
                {
                    string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    ViewBag.AlertMessage = responseData;
                    ViewBag.AlertType = "alert-success";
                    ViewBag.Display = "block;";
                }
            }

            TempData["AlertType"] = ViewBag.AlertType;
            TempData["AlertMessage"] = ViewBag.AlertMessage;
            TempData["Display"] = ViewBag.AlertMessage;

            return RedirectToAction("viewBusRoutes", listBusRoutes);
        }

        public async Task<List<BusRouteView>> getBusRoutes(int id)
        {
            List<BusRouteView> listBusRoutes = await busRouteService.GetBusRoutesAsync(id);
            return listBusRoutes;
        }

    }
}