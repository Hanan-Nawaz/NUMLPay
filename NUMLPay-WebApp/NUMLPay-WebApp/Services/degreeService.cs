using Newtonsoft.Json;
using NUMLPay_WebApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Services
{
    public class degreeService
    {
        private readonly string _apiBaseUrl;
        apiService apiServices;

        public degreeService(Uri baseUrl)
        {
            _apiBaseUrl = baseUrl.ToString();
            apiServices = new apiService(_apiBaseUrl);
        }

        //Get Degree on base of Dept and Academic Level
        public async Task<List<SelectListItem>> getDegrees(int deptId, int levelId, int? selectedValue)
        {
            int deptIdValue = Convert.ToInt32(deptId);
            int levelIdValue = Convert.ToInt32(levelId);

            List<SelectListItem> shiftOptions = new List<SelectListItem>();

            List<DegreeView> listDegree = null;
            HttpResponseMessage responseMessage = await apiServices.GetAsync($"Degree/GetDegreebyDeptLevel/{deptIdValue}/{levelIdValue}");

            if (responseMessage.IsSuccessStatusCode)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                listDegree = JsonConvert.DeserializeObject<List<DegreeView>>(responseData);
            }

            foreach (var shifts in listDegree)
            {
                shiftOptions.Add(new SelectListItem { Value = shifts.SId.ToString(), Text = shifts.name.ToString() });
            }

            if (selectedValue.HasValue)
            {
                foreach (var option in shiftOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return shiftOptions;
        }
    }
}