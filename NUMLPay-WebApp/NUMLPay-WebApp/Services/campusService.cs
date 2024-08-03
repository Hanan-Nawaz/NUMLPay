using Newtonsoft.Json;
using NUMLPay_WebApp.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Web.Mvc;
using System;
using System.Runtime.CompilerServices;
using System.Linq;

namespace NUMLPay_WebApp.Services
{
    public class CampusService
    {
        private readonly string _apiBaseUrl;
        apiService apiServices;

        public CampusService(Uri baseUrl)
        {
            _apiBaseUrl = baseUrl.ToString();
            apiServices = new apiService(_apiBaseUrl);
        }

        //select camous by Id
        public async Task<List<SelectListItem>> getSelectedCampus(int? selectedValue)
        {
            List<Campus> campusList = await getCampusAsync();
            List<SelectListItem> campusOptions = campusList
                    .Select(campus => new SelectListItem
                    {
                        Value = campus.Id.ToString(),
                        Text = campus.name 
                    })
                    .ToList();

            if (selectedValue.HasValue)
            {
                foreach (var option in campusOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return campusOptions;
        }

        //Add Campus to SelectList
        public async Task<SelectList> addCampustoListAsync()
        {
            List<Campus> listCampus = await getActiveCampusAsync();
            SelectList campusList = new SelectList(listCampus, "id", "name");

            return campusList;
        }


        // Get only active Campus
        public async Task<List<Campus>> getActiveCampusAsync()
        {
            List<Campus> activeCampusList = new List<Campus>();
            List<Campus> allCampusList = await getCampusAsync();

            if (allCampusList != null)
            {
                foreach (var campus in allCampusList)
                {
                    if (campus.is_active == 1)
                    {
                        activeCampusList.Add(campus);
                    }
                }
            }

            return activeCampusList;
        }

        // Get all Campus
        public async Task<List<Campus>> getCampusAsync()
        {
            List<Campus> listCampus = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}Campus"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listCampus = JsonConvert.DeserializeObject<List<Campus>>(data);
                }
            }

            return listCampus;
        }

        // Get Campus by Id
        public async Task<Campus> getCampusAsync(int Id)
        {
            Campus campus = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}/Campus/GetbyId/{Id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    campus = JsonConvert.DeserializeObject<Campus>(data);
                }
            }

            return campus;
        }
    }
}
