using Newtonsoft.Json;
using NUMLPay_WebApp.Models;
using NUMLPay_WebApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Services
{
    public class summerFeeService
    {
        private readonly string _apiBaseUrl;
        apiService apiServices;

        public summerFeeService(Uri baseUrl)
        {
            _apiBaseUrl = baseUrl.ToString();
            apiServices = new apiService(_apiBaseUrl);
        }

        // Get all SummerFees
        public async Task<List<SummerFeeView>> getSummerFeesAsync(int id)
        {
            List<SummerFeeView> listSummerFees = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}SummerFee/GetByDeptId/{id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listSummerFees = JsonConvert.DeserializeObject<List<SummerFeeView>>(data);
                }
            }

            return listSummerFees;
        }

        public async Task<List<SummerFeeView>> GetActiveSummerFeesForYearAsync(int id)
        {
            List<SummerFeeView> activeSummerFees = new List<SummerFeeView>();
            List<SummerFeeView> allSummerFees = await getSummerFeesAsync(id);

            if (allSummerFees != null)
            {
                int currentYear = DateTime.Now.Year;

                foreach (var summerFee in allSummerFees)
                {
                    if (summerFee.is_active == 1 && summerFee.session_year == currentYear)
                    {
                        activeSummerFees.Add(summerFee);
                    }
                }
            }

            return activeSummerFees;
        }


        public async Task<List<SummerFee>> getSummerFeeAsync()
        {
            List<SummerFee> listSummerFees = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}SummerFee"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listSummerFees = JsonConvert.DeserializeObject<List<SummerFee>>(data);
                }
            }

            return listSummerFees;
        }

        // Get SummerFee by Id
        public async Task<SummerFee> getSummerFeeAsync(int Id)
        {
            SummerFee summerFee = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}/SummerFee/GetById/{Id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    summerFee = JsonConvert.DeserializeObject<SummerFee>(data);
                }
            }

            return summerFee;
        }

        // Get only active SummerFees
        public async Task<List<SummerFee>> getActiveSummerFeesAsync()
        {
            List<SummerFee> activeSummerFeeList = new List<SummerFee>();
            List<SummerFee> allSummerFeeList = await getSummerFeeAsync();

            if (allSummerFeeList != null)
            {
                foreach (var summerFee in allSummerFeeList)
                {
                    if (summerFee.is_active == 1)
                    {
                        activeSummerFeeList.Add(summerFee);
                    }
                }
            }

            return activeSummerFeeList;
        }

        // Select SummerFee by Id
        public async Task<List<SelectListItem>> getSelectedSummerFee(int? selectedValue)
        {
            List<SummerFee> summerFeeList = await getSummerFeeAsync();
            List<SelectListItem> summerFeeOptions = summerFeeList
                    .Select(summerFee => new SelectListItem
                    {
                        Value = summerFee.id.ToString(),
                        Text = summerFee.fee.ToString()
                    })
                    .ToList();

            if (selectedValue.HasValue)
            {
                foreach (var option in summerFeeOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return summerFeeOptions;
        }
    }
}
