using Newtonsoft.Json;
using NUMLPay_WebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Services
{
    public class feePlanService
    {
        private readonly string _apiBaseUrl;
        apiService apiServices;

        public feePlanService(Uri baseUrl)
        {
            _apiBaseUrl = baseUrl.ToString();
            apiServices = new apiService(_apiBaseUrl);
        }

        //Get all Fee Plan
        public async Task<List<FeePlan>> getFeePlanAsync()
        {
            List<FeePlan> listFee = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}FeePlan/Get"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listFee = JsonConvert.DeserializeObject<List<FeePlan>>(data);
                }
            }
            return listFee;
        }

        // Add Fee Plan to SelectList
        public async Task<SelectList> addFeePlantoListAsync(int? selectedValue)
        {
            List<FeePlan> listFee = await getFeePlanAsync();
            List<SelectListItem> feeOptions = new List<SelectListItem>();

            foreach (var fee in listFee)
            {
                feeOptions.Add(new SelectListItem { Value = fee.id.ToString(), Text = $"{fee.name} - {fee.discount}%" });
            }

            if (selectedValue.HasValue)
            {
                foreach (var option in feeOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return new SelectList(feeOptions, "Value", "Text");
        }


        // Get FeePlan by Id
        public async Task<FeePlan> getFeePlanAsync(int Id)
        {
            FeePlan feePlan = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}FeePlan/GetById/{Id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    feePlan = JsonConvert.DeserializeObject<FeePlan>(data);
                }
            }

            return feePlan;
        }

        //All Fees
        public async Task<List<SelectListItem>> getFeeForUser(Users user)
        {
            EligibleFees eFees = new EligibleFees();
            eFees.std_numl_id = user.numl_id;
            using (var response = await apiServices.PostAsync($"{_apiBaseUrl}Users/GetEligibleFees", eFees))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    eFees = JsonConvert.DeserializeObject<EligibleFees>(data);
                }
            }


            List<SelectListItem> feeOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Tuition Fee" },
            };

            if (eFees.bus_fee == 1)
            {
                feeOptions.Add(new SelectListItem { Value = "2", Text = "Bus Fee" });
            }

            if(eFees.hostel_fee == 1)
            {
                feeOptions.Add(new SelectListItem { Value = "3", Text = "Hostel Fee" });
                feeOptions.Add(new SelectListItem { Value = "4", Text = "Mess Fee" });
            }

            return feeOptions;
        }

    }
}