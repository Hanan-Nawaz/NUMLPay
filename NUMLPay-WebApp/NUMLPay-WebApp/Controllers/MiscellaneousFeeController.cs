using Newtonsoft.Json;
using NUMLPay_WebApp.Models;
using NUMLPay_WebApp.Services;
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
    public class MiscellaneousFeeController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        descriptionService descService;
        miscellaneousFeeService mFeeService;
        usersTypeService usersTypeService;
        statusService StatusService;
        apiService apiServices;

        public MiscellaneousFeeController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            descService = new descriptionService(baseAddress);
            mFeeService = new miscellaneousFeeService(baseAddress);
            usersTypeService = new usersTypeService();
            StatusService = new statusService();
            apiServices = new apiService(baseAddress.ToString());
        }

        // Add MiscellaneousFee
        public async Task<ActionResult> addMiscellaneousFee()
        {
            userAccessAdmin();

            ViewBag.descList = await descService.addDescriptionToListAsync();

            ViewBag.Display = "none;";
            return View();
        }

        // Add MiscellaneousFee Post
        [HttpPost]
        public async Task<ActionResult> addMiscellaneousFee(MiscellaneousFees fee)
        {
            Admin admin = userAccessAdmin();

            ViewBag.descList = await descService.addDescriptionToListAsync();

            fee.added_by = admin.email_id;
            fee.is_active = 1;

            HttpResponseMessage responseMessage = await apiServices.PostAsync("MiscellaneousFees", fee);

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

        // View all Miscellaneous Fees
        public async Task<ActionResult> viewMiscellaneousFees()
        {
            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            await getInDictionary(); 

            userAccessAdmin();

            List<MiscellaneousFees> listFees = await mFeeService.getMiscellaneousFeesAsync(); 

            return View(listFees);
        }

        // Update Miscellaneous Fee
        public async Task<ActionResult> updateMiscellaneousFee(int Id)
        {
            userAccessAdmin();

            MiscellaneousFees fee = await mFeeService.getMiscellaneousFeeAsync(Id);

            ViewBag.Display = "none;";

            int? selectedValue = fee.is_active;

            ViewBag.descList = new SelectList(await descService.getSelectedDescription(fee.desc_id), "Value", "Text");
            ViewBag.is_active = StatusService.getStatus(selectedValue);

            return View(fee);
        }

        // Update Miscellaneous Fee (Post)
        [HttpPost]
        public async Task<ActionResult> updateMiscellaneousFee(MiscellaneousFees fee)
        {
            Admin admin = userAccessAdmin();

            ViewBag.descList = new SelectList(await descService.getSelectedDescription(fee.desc_id), "Value", "Text");

            ViewBag.Display = "none;";

            fee.added_by = admin.email_id;

            HttpResponseMessage responseMessage = await apiServices.PutAsync($"MiscellaneousFees/updateMiscellaneousFee/{fee.Id}", fee);

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

            List<MiscellaneousFees> listFees = await mFeeService.getMiscellaneousFeesAsync();
            await getInDictionary();
            return View("viewMiscellaneousFees", listFees);
        }

        // Delete Miscellaneous Fee
        public async Task<ActionResult> deleteMiscellaneousFee(int? Id)
        {
            Admin admin = userAccessAdmin();

            List<MiscellaneousFees> listFees = await mFeeService.getMiscellaneousFeesAsync();

            if (!Id.HasValue)
            {
                return RedirectToAction("viewMiscellaneousFees");
            }
            else
            {
                MiscellaneousFees fee = new MiscellaneousFees();
                fee.is_active = 2;

                HttpResponseMessage responseMessage = await apiServices.PutAsync($"MiscellaneousFees/inActive/{Id}", fee);

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

            return RedirectToAction("viewMiscellaneousFees", listFees);
        }


        public async Task<bool> getInDictionary()
        {
            var descList = await descService.getDescAsync();
            ViewBag.descDictionary = descList.ToDictionary(d => d.id.ToString(), c => c.name);
            return true;
        }
    }
}