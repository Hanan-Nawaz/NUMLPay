using Newtonsoft.Json;
using NUMLPay_WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Services
{
    public class installmentService
    {
        private readonly string _apiBaseUrl;
        apiService apiServices;

        public installmentService(Uri baseUrl)
        {
            _apiBaseUrl = baseUrl.ToString();
            apiServices = new apiService(_apiBaseUrl);
        }

        //get Installments Mode
        public List<SelectListItem> getNumberOfInstallments(int? selectedValue)
        {
            List<SelectListItem> installmentOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "No Installment" },
                new SelectListItem { Value = "2", Text = "2 Installments" },
                new SelectListItem { Value = "3", Text = "3 Installments" },
                new SelectListItem { Value = "4", Text = "4 Installments" }
            };

            if (selectedValue.HasValue)
            {
                foreach (var option in installmentOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return installmentOptions;
        }


        // Select Installment by Id
        public async Task<List<SelectListItem>> getSelectedInstallment(int? selectedValue)
        {
            List<InstallmentManagement> listInstallments = await getInstallmentsAsync();

            List<SelectListItem> installmentOptions = listInstallments
                .Select(inst => new SelectListItem
                {
                    Value = inst.installment_id.ToString(),
                    Text = inst.mode.ToString()
                })
                .ToList();

            if (selectedValue.HasValue)
            {
                foreach (var option in installmentOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return installmentOptions;
        }

        // Add Installment to SelectList
        public async Task<SelectList> addInstallmentToListAsync(int feeFor)
        {
            List<InstallmentManagement> listInstallments = await getActiveInstallmentsAsync(feeFor);
            List<SelectListItem> sessionOptions = new List<SelectListItem>();

            foreach (var session in listInstallments)
            {
                string sessionText = $"{getName(session.mode)}";
                sessionOptions.Add(new SelectListItem { Value = session.installment_id.ToString(), Text = sessionText });
            }

            SelectList installmentList = new SelectList(sessionOptions, "Value", "Text");

            return installmentList;
        }

        // Helper method to get  name based on its value
        private string getName(int value)
        {
            switch (value)
            {
                case 1:
                    return "No Installments";
                case 2:
                    return "2 Installments";
                case 3:
                    return "3 Installments";
                case 4:
                    return "4 Installments";
                default:
                    return "unknown";
            }
        }

        // Get all Installments
        public async Task<List<InstallmentManagement>> getInstallmentsAsync()
        {
            List<InstallmentManagement> listInstallments = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}InstallmentManagement"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listInstallments = JsonConvert.DeserializeObject<List<InstallmentManagement>>(data);
                }
            }

            return listInstallments;
        }

        // Get active Installments
        public async Task<List<InstallmentManagement>> getActiveInstallmentsAsync(int fee_for)
        {
            List<InstallmentManagement> listInstallments = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}InstallmentManagement/getActive/{fee_for}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listInstallments = JsonConvert.DeserializeObject<List<InstallmentManagement>>(data);
                }
            }

            return listInstallments;
        }

        // Get Installment by Id
        public async Task<InstallmentManagement> getInstallmentAsync(int Id)
        {
            InstallmentManagement installment = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}InstallmentManagement/{Id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    installment = JsonConvert.DeserializeObject<InstallmentManagement>(data);
                }
            }

            return installment;
        }


    }

}