using Microsoft.Ajax.Utilities;
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
    [CustomAuthorizationFilter(requireAdmin: true, requiredRole: new int[] { 2, 4 })]
    public class OtherFeeController : SessionController
    {

        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        CampusService campusService;
        usersTypeService usersTypeService;
        statusService StatusService;
        busRouteService busRouteService;
        departmentService departmentServices;
        academicLevelService levelService;
        sessionService sessionServices;
        feePlanService feeService;
        degreeService degreeServices;
        apiService apiServices;

        public OtherFeeController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            campusService = new CampusService(baseAddress);
            usersTypeService = new usersTypeService();
            StatusService = new statusService();
            busRouteService = new busRouteService(baseAddress);
            departmentServices = new departmentService(baseAddress);
            levelService = new academicLevelService();
            sessionServices = new sessionService(baseAddress);
            feeService = new feePlanService(baseAddress);
            degreeServices = new degreeService(baseAddress);
            apiServices = new apiService(baseAddress.ToString());
        }

        //add Fee
        public async Task<ActionResult> addOtherFee()
        {
            Admin admin = userAccessAdmin();

            ViewBag.Display = "none;";

            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(null);

            ViewBag.feeFor = new SelectList(StatusService.getOtherFeeFor(null), "value", "text");
            ViewBag.campusList = await campusService.addCampustoListAsync();
            ViewBag.route = "";

            return View();
        }

        //add Fee
        [HttpPost]
        public async Task<ActionResult> addOtherFee(FeeStructure feeStructure)
        {
            Admin admin = userAccessAdmin();

            ViewBag.Display = "none;";

            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(null);

            ViewBag.feeFor = new SelectList(StatusService.getOtherFeeFor(null), "value", "text");

            FeeSecurity newfeeSecurity = new FeeSecurity();
            feeStructure.added_by = admin.email_id;
            int securityFee = 0, busRoute = 0, campus;

            if(feeStructure.fee_for == 2)
            {
                busRoute = Convert.ToInt32(Request.Form["routeDropdown"]);
            }
            campus = Convert.ToInt32(Request.Form["campusDropdown"]);

            if (feeStructure.fee_for != 2)
            {
                feeStructure.shift_id = campus;

                if (!int.TryParse(Request.Form["securityFee"], out securityFee))
                {
                    ViewBag.AlertMessage = "Please Enter Complete Fee Data";
                    ViewBag.AlertType = "alert-danger";
                    ViewBag.Display = "block;";
                    return View(feeStructure);
                }
                securityFee = Convert.ToInt32(Request.Form["securityFee"]);
            }

            if(feeStructure.fee_for == 2)
            {
                feeStructure.shift_id = busRoute;
            }

                newfeeSecurity.security = securityFee;

                HttpResponseMessage responseMessageFees = await apiServices.PostAsync("FeeStructure", feeStructure);

                if (responseMessageFees.IsSuccessStatusCode)
                {
                    string data = await responseMessageFees.Content.ReadAsStringAsync();
                    newfeeSecurity.fee_structure_id = Convert.ToInt32(data);

                    if (feeStructure.fee_for == 3 || feeStructure.fee_for == 4)
                    {
                        newfeeSecurity.fee_structure_id = Convert.ToInt32(data);
                        await addFeeSecurity(newfeeSecurity);
                    }
                    else
                    {
                        ViewBag.AlertMessage = "Fee Structure Added Successfully!";
                        ViewBag.AlertType = "alert-success";
                        ViewBag.Display = "block;";
                    }
                }
                else
                {
                    string responseData = responseMessageFees.Content.ReadAsStringAsync().Result;
                    ViewBag.AlertMessage = responseData;
                    ViewBag.AlertType = "alert-danger";
                    ViewBag.Display = "block;";
                }

            return View();
        }

        // View all Fees
        public async Task<ActionResult> viewOtherFees()
        {
            Admin admin = userAccessAdmin();

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            List<FeeView> listFee  = await giveRealtiveData();

            return View(listFee);
        }

        // Update Fee
        public async Task<ActionResult> updateOtherFee(int Id)
        {
            Admin admin = userAccessAdmin();

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            FeeStructure fee = null;
            FeeSecurity sec = null;

            using (var responses = await apiServices.GetAsync($"FeeStructure/GetbyId/{Id}"))
            {
                if (responses.IsSuccessStatusCode)
                {
                    string data = await responses.Content.ReadAsStringAsync();
                    fee = JsonConvert.DeserializeObject<FeeStructure>(data);
                }
            }

            ViewBag.feeFor = new SelectList(StatusService.getFeeFor(fee.fee_for), "value", "text");

            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(Convert.ToInt32(fee.session));

            using (var responses = await apiServices.GetAsync($"FeeSecurity/GetByFeeId/{Id}"))
            {
                if (responses.IsSuccessStatusCode)
                {
                    string data = await responses.Content.ReadAsStringAsync();
                    sec = JsonConvert.DeserializeObject<FeeSecurity>(data);
                }
            }
            if (sec == null)
            {
                ViewBag.securityFee = 0;

            }
            else
            {
                ViewBag.securityFee = sec.security;
            }

            return View(fee);
        }

        // Update Fee Post
        [HttpPost]
        public async Task<ActionResult> updateOtherFee(FeeStructure feeStructure)
        {
            Admin admin = userAccessAdmin();

            ViewBag.admissionSession = await sessionServices.addSessiontoListAsync(feeStructure.session);

            ViewBag.feeFor = new SelectList(StatusService.getFeeFor(feeStructure.fee_for), "value", "text");

            FeeSecurity newfeeSecurity = new FeeSecurity();
            feeStructure.added_by = admin.email_id;
            int securityFee;

            if (!int.TryParse(Request.Form["securityFee"], out securityFee) && (feeStructure.fee_for == 3 || feeStructure.fee_for == 4))
            {
                ViewBag.AlertMessage = "Please Enter Complete Fee Data";
                ViewBag.AlertType = "alert-danger";
                ViewBag.Display = "block;";
                return View(feeStructure);
            }
            else
            {
                newfeeSecurity.security = securityFee;

                HttpResponseMessage responseMessageFees = await apiServices.PutAsync("FeeStructure", feeStructure);

                if (responseMessageFees.IsSuccessStatusCode)
                {
                    string data = await responseMessageFees.Content.ReadAsStringAsync();
                    newfeeSecurity.fee_structure_id = feeStructure.Id;

                    if (feeStructure.fee_for == 3 || feeStructure.fee_for == 4)
                    {
                        HttpResponseMessage responseMessage = await apiServices.PutAsync("FeeSecurity", newfeeSecurity);
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
                    }
                    else
                    {
                        ViewBag.AlertMessage = "Fee Structure Added Successfully!";
                        ViewBag.AlertType = "alert-success";
                        ViewBag.Display = "block;";
                    }
                }
                else
                {
                    string responseData = responseMessageFees.Content.ReadAsStringAsync().Result;
                    ViewBag.AlertMessage = responseData;
                    ViewBag.AlertType = "alert-danger";
                    ViewBag.Display = "block;";
                }

            }

            List<FeeView> listFee = await giveRealtiveData();

            TempData["AlertType"] = ViewBag.AlertType;
            TempData["AlertMessage"] = ViewBag.AlertMessage;
            TempData["Display"] = ViewBag.AlertMessage;

            return RedirectToAction("viewOtherFees", listFee);
        }

        //get Fees
        public async Task<List<FeeView>> giveRealtiveData()
        {
            List<FeeView> feeList = null;

                var response = await apiServices.GetAsync($"FeeStructure/GetHostelBusMess");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    feeList = JsonConvert.DeserializeObject<List<FeeView>>(data);
                }

            return feeList;
        }

        //adding FeeSecurity
        private async Task<bool> addFeeSecurity(FeeSecurity feeSecurity)
        {
            HttpResponseMessage responseMessage = await apiServices.PostAsync("FeeSecurity", feeSecurity);
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
            return true;
        }

        public async Task<ActionResult> getRoutes(int campus_id)
        {
            List<SelectListItem> shiftOptions = new List<SelectListItem>();

            List<BusRoute> listBusRoute = null;
            HttpResponseMessage responseMessage = await apiServices.GetAsync($"BusRoute/GetByCampusId/{campus_id}");

            if (responseMessage.IsSuccessStatusCode)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                listBusRoute = JsonConvert.DeserializeObject<List<BusRoute>>(responseData);
            }
            if (listBusRoute != null)
            {
                foreach (var shifts in listBusRoute)
                {
                    shiftOptions.Add(new SelectListItem { Value = shifts.id.ToString(), Text = shifts.name.ToString() });
                }
            }

            return Json(shiftOptions, JsonRequestBehavior.AllowGet);
        }


    }

}