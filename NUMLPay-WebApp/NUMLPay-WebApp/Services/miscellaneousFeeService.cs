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
    public class miscellaneousFeeService
    {
        private readonly string _apiBaseUrl;
        apiService apiServices;

        public miscellaneousFeeService(Uri baseUrl)
        {
            _apiBaseUrl = baseUrl.ToString();
            apiServices = new apiService(_apiBaseUrl);
        }

        // Get all Miscellaneous Fees
        public async Task<List<MiscellaneousFees>> getMiscellaneousFeesAsync()
        {
            List<MiscellaneousFees> listMiscellaneousFees = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}MiscellaneousFees"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listMiscellaneousFees = JsonConvert.DeserializeObject<List<MiscellaneousFees>>(data);
                }
            }

            return listMiscellaneousFees;
        }

        // Get all Miscellaneous Fees
        public async Task<List<dynamic>> getMiscellaneousFeesAvtiveAsync()
        {
            List<dynamic> listMiscellaneousFees = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}MiscellaneousFees/Active"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listMiscellaneousFees = JsonConvert.DeserializeObject<List<dynamic>>(data);
                }
            }

            return listMiscellaneousFees;
        }

        // Select Miscellaneous Fees by Id
        public async Task<List<SelectListItem>> getSelectedMiscellaneousFees(int? selectedValue)
        {
            List<dynamic> listMiscellaneousFees = await getMiscellaneousFeesAvtiveAsync();

            List<SelectListItem> feeOptions = listMiscellaneousFees
                .Select(fee => new SelectListItem
                {
                    Value = fee.Id.ToString(),
                    Text = fee.name.ToString()
                })
                .ToList();

            if (selectedValue.HasValue)
            {
                foreach (var option in feeOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return feeOptions;
        }


        // Get Miscellaneous Fee by Id
        public async Task<MiscellaneousFees> getMiscellaneousFeeAsync(int id)
        {
            MiscellaneousFees fee = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}/MiscellaneousFees/{id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    fee = JsonConvert.DeserializeObject<MiscellaneousFees>(data);
                }
            }

            return fee;
        }

    }
}