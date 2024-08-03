using Newtonsoft.Json;
using NUMLPay_WebApp.Models;
using NUMLPay_WebApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Services
{
    public class busRouteService
    {
        private readonly string _apiBaseUrl;
        private readonly apiService _apiServices;

        public busRouteService(Uri baseUrl)
        {
            _apiBaseUrl = baseUrl.ToString();
            _apiServices = new apiService(_apiBaseUrl);
        }

        // Select Bus Route by Id
        public async Task<List<SelectListItem>> GetSelectedBusRoute(int? selectedValue, int id)
        {
            List<BusRoute> busRoutes = await GetBusRoutesActiveAsync(id);
            List<SelectListItem> busRouteOptions = busRoutes
                .Select(busRoute => new SelectListItem
                {
                    Value = busRoute.id.ToString(),
                    Text = busRoute.name
                })
                .ToList();

            if (selectedValue.HasValue)
            {
                foreach (var option in busRouteOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return busRouteOptions;
        }

        public async Task<List<SelectListItem>> GetSelectedBusRouteAdmin(int? selectedValue, int id)
        {
            List<BusRoute> busRoutes = await GetActiveBusRoutesAsync(id);
            List<SelectListItem> busRouteOptions = busRoutes
                .Select(busRoute => new SelectListItem
                {
                    Value = busRoute.id.ToString(),
                    Text = busRoute.name
                })
                .ToList();

            if (selectedValue.HasValue)
            {
                foreach (var option in busRouteOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return busRouteOptions;
        }

        // Get all Bus Routes
        public async Task<List<BusRouteView>> GetBusRoutesAsync(int id)
        {
            List<BusRouteView> busRoutes = null;
            using (var response = await _apiServices.GetAsync($"{_apiBaseUrl}/BusRoute/GetByCampusId/{id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    busRoutes = JsonConvert.DeserializeObject<List<BusRouteView>>(data);
                }
            }

            return busRoutes;
        }

        // Get all Bus Routes
        public async Task<List<BusRoute>> GetActiveBusRoutesAsync(int id)
        {
            List<BusRoute> busRoutes = null;
            using (var response = await _apiServices.GetAsync($"{_apiBaseUrl}/BusRoute/GetActivebyCampus/{id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    try
                    {
                        busRoutes = JsonConvert.DeserializeObject<List<BusRoute>>(data);
                    }
                    catch (JsonException ex)
                    {
                        // Handle JSON deserialization errors
                        Console.WriteLine("Error deserializing JSON: " + ex.Message);
                    }
                }
            }

            return busRoutes;
        }

        // Get all Bus Routes
        public async Task<List<BusRoute>> GetBusRoutesActiveAsync(int id)
        {
            List<BusRoute> busRoutes = null;
            using (var response = await _apiServices.GetAsync($"{_apiBaseUrl}/BusRoute/GetActiveForDepartment/{id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    busRoutes = JsonConvert.DeserializeObject<List<BusRoute>>(data);
                }
            }

            return busRoutes;
        }
    }
}
