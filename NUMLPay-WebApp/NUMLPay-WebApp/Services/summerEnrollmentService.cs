using Newtonsoft.Json;
using NUMLPay_WebApp.Models;
using NUMLPay_WebApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NUMLPay_WebApp.Services
{
    public class summerEnrollmentService
    {
        private readonly string _apiBaseUrl;
        apiService apiServices;

        public summerEnrollmentService(Uri baseUrl)
        {
            _apiBaseUrl = baseUrl.ToString();
            apiServices = new apiService(_apiBaseUrl);
        }

        // Get all SummerEnrollments
        public async Task<List<SummerEnrollmentView>> getSummerEnrollmentsAsync(int id)
        {
            List<SummerEnrollmentView> listSummerEnrollments = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}SummerEnrollment/GetByDeptId/{id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listSummerEnrollments = JsonConvert.DeserializeObject<List<SummerEnrollmentView>>(data);
                }
            }

            return listSummerEnrollments;
        }

        // Get SummerEnrollment by Id
        public async Task<SummerEnrollment> getSummerEnrollmentAsync(int Id)
        {
            SummerEnrollment summerEnrollment = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}/SummerEnrollment/GetById/{Id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    summerEnrollment = JsonConvert.DeserializeObject<SummerEnrollment>(data);
                }
            }

            return summerEnrollment;
        }
    }
}