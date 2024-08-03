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
    public class descriptionService
    {
        private readonly string _apiBaseUrl;
        apiService apiServices;

        public descriptionService(Uri baseUrl)
        {
            _apiBaseUrl = baseUrl.ToString();
            apiServices = new apiService(_apiBaseUrl);
        }

        //select Desc by Id
        public async Task<List<SelectListItem>> getSelectedDescription(int? selectedValue)
        {
            List<Description> listDescription = await getDescAsync();

            List<SelectListItem> descOptions = listDescription
                    .Select(desc => new SelectListItem
                    {
                        Value = desc.id.ToString(),
                        Text = desc.name
                    })
                    .ToList();

            if (selectedValue.HasValue)
            {
                foreach (var option in descOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return descOptions;
        }

        // Add Description to SelectList
        public async Task<SelectList> addDescriptionToListAsync()
        {
            List<Description> listDescription = await getActiveDescAsync();
            SelectList descriptionList = new SelectList(listDescription, "id", "name");

            return descriptionList;
        }


        // Get all Descs
        public async Task<List<Description>> getDescAsync()
        {
            List<Description> listDescription = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}Description"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listDescription = JsonConvert.DeserializeObject<List<Description>>(data);
                }
            }

            return listDescription;
        }

        // Get all Descs
        public async Task<List<Description>> getActiveDescAsync()
        {
            List<Description> listDescription = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}Description/getActive"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listDescription = JsonConvert.DeserializeObject<List<Description>>(data);
                }
            }

            return listDescription;
        }


        // Get Description by Id
        public async Task<Description> getDescriptionAsync(int Id)
        {
            Description description = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}/Description/{Id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    description = JsonConvert.DeserializeObject<Description>(data);
                }
            }

            return description;
        }

    }
}