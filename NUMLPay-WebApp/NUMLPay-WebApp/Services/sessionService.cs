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
    public class sessionService
    {
        private readonly string _apiBaseUrl;
        apiService apiServices;

        public sessionService(Uri baseUrl)
        {
            _apiBaseUrl = baseUrl.ToString();
            apiServices = new apiService(_apiBaseUrl);
        }

        //Get all Session's
        public async Task<List<Session>> getSessionAsync()
        {
            List<Session> listSession = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}Session/Get"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listSession = JsonConvert.DeserializeObject<List<Session>>(data);
                }
            }
            return listSession;
        }


        //Get all Session's
        public async Task<List<SeesionView>> getSessionAsyncforDDl()
        {
            List<SeesionView> listSession = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}Session/GetforDdl"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listSession = JsonConvert.DeserializeObject<List<SeesionView>>(data);
                }
            }
            return listSession;
        }

        // All Session Statuses
        public List<SelectListItem> getSessionName(int? selectedValue)
        {
            List<SelectListItem> statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Fall" },
                new SelectListItem { Value = "2", Text = "Spring" }
            };

            if (selectedValue.HasValue)
            {
                foreach (var option in statusOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return statusOptions;
        }


        // Add Session to SelectList
        public async Task<SelectList> addSessiontoListAsync(int? selectedValue)
        {
            List<SeesionView> listSession = await getSessionAsyncforDDl();
            List<SelectListItem> sessionOptions = new List<SelectListItem>();

            foreach (var session in listSession)
            {
                sessionOptions.Add(new SelectListItem { Value = session.id.ToString(), Text = session.session.ToString() });
            }

            if (selectedValue.HasValue)
            {
                foreach (var option in sessionOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return new SelectList(sessionOptions, "Value", "Text");
        }

        // Add Session to SelectList
        public async Task<SelectList> addSessiontoListAsyncWithEligibility(int sessionId)
        {
            List<SeesionView> listSession = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}Session/GetforEligibleDdl/{sessionId}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listSession = JsonConvert.DeserializeObject<List<SeesionView>>(data);
                }
            }

            List<SelectListItem> sessionOptions = new List<SelectListItem>();

            foreach (var session in listSession)
            {
                sessionOptions.Add(new SelectListItem { Value = session.id.ToString(), Text = session.session.ToString() });
            }

            

            return new SelectList(sessionOptions, "Value", "Text");
        }

        // Get Session by Id
        public async Task<Session> getSessionAsync(int Id)
        {
            Session session = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}Session/GetById/{Id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    session = JsonConvert.DeserializeObject<Session>(data);
                }
            }

            return session;
        }

        // Method to get all years from 1990 to the current year
        public List<SelectListItem> getYearsFrom1990ToCurrent()
        {
            List<SelectListItem> yearOptions = new List<SelectListItem>();
            int currentYear = DateTime.Now.Year;

            for (int year = currentYear; year >= 1990; year--)
            {
                yearOptions.Add(new SelectListItem
                {
                    Value = year.ToString(),
                    Text = year.ToString()
                });
            }

            return yearOptions;
        }
    }
}