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
    public class InstallmentManagementController : SessionController
    {
        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]);
        HttpClient httpClient;
        installmentService installmentServices;
        usersTypeService usersTypeService;
        statusService StatusService;
        apiService apiServices;

        public InstallmentManagementController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = baseAddress;
            installmentServices = new installmentService(baseAddress);
            usersTypeService = new usersTypeService();
            StatusService = new statusService();
            apiServices = new apiService(baseAddress.ToString());
        }

        // Add Installment
        public async Task<ActionResult> addInstallment()
        {
            userAccessAdmin();
            ViewBag.Display = "none;";
            ViewBag.feeFor = new SelectList(StatusService.getFeeFor(null), "value", "text");
            ViewBag.mode = new SelectList(installmentServices.getNumberOfInstallments(null), "value", "text");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> addInstallment(InstallmentManagement installment)
        {
            Admin admin = userAccessAdmin();
            installment.is_active = 1;
            installment.added_by = admin.email_id;

            ViewBag.feeFor = new SelectList(StatusService.getFeeFor(null), "value", "text");
            ViewBag.mode = new SelectList(installmentServices.getNumberOfInstallments(null), "value", "text");

            HttpResponseMessage responseMessage = await apiServices.PostAsync("InstallmentManagement", installment);

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

        // View all Installments
        public async Task<ActionResult> viewInstallments()
        {
            userAccessAdmin();

            List<InstallmentManagement> listInstallments = await installmentServices.getInstallmentsAsync();

            ViewBag.AlertType = TempData["AlertType"]?.ToString() ?? "";
            ViewBag.AlertMessage = TempData["AlertMessage"]?.ToString() ?? "";
            ViewBag.Display = TempData["Display"] ?? "none;";

            return View(listInstallments);
        }

        // Update Installment
        public async Task<ActionResult> updateInstallment(int Id)
        {
            userAccessAdmin();
            InstallmentManagement installment = await installmentServices.getInstallmentAsync(Id);
            ViewBag.Display = "none;";

            ViewBag.feeFor = new SelectList(StatusService.getFeeFor(installment.fee_for), "value", "text");
            ViewBag.mode = installmentServices.getNumberOfInstallments(installment.mode);

            int? selectedValue = installment.is_active;

            ViewBag.is_active = StatusService.getStatus(selectedValue);

            return View(installment);
        }

        [HttpPost]
        public async Task<ActionResult> updateInstallment(InstallmentManagement installment)
        {
            Admin admin = userAccessAdmin();
            ViewBag.Display = "none;";
            installment.is_active = 1;
            installment.added_by = admin.email_id;

            ViewBag.feeFor = new SelectList(StatusService.getFeeFor(installment.fee_for), "value", "text");
            ViewBag.mode = installmentServices.getNumberOfInstallments(installment.mode);

            HttpResponseMessage responseMessage = await apiServices.PutAsync($"InstallmentManagement/{installment.installment_id}", installment);

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

            List<InstallmentManagement> listInstallments = await installmentServices.getInstallmentsAsync();
            TempData["AlertType"] = ViewBag.AlertType;
            TempData["AlertMessage"] = ViewBag.AlertMessage;
            TempData["Display"] = ViewBag.AlertMessage;
            return View("ViewInstallments", listInstallments);
        }

        // Delete Installment
        public async Task<ActionResult> deleteInstallment(int? Id)
        {
            userAccessAdmin();

            if (!Id.HasValue)
            {
                return RedirectToAction("ViewInstallments");
            }
            else
            {
                InstallmentManagement installment = new InstallmentManagement();
                installment.is_active = 2;

                HttpResponseMessage responseMessage = await apiServices.PutAsync($"InstallmentManagement/inActive/{Id}", installment);

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

            List<InstallmentManagement> listInstallments = await installmentServices.getInstallmentsAsync();

            TempData["AlertType"] = ViewBag.AlertType;
            TempData["AlertMessage"] = ViewBag.AlertMessage;
            TempData["Display"] = ViewBag.AlertMessage;

            return RedirectToAction("ViewInstallments", listInstallments);
        }
    }
}