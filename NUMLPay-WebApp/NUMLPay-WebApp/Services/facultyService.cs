using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using NUMLPay_WebApp.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using NUMLPay_WebApp.ViewModel;

namespace NUMLPay_WebApp.Services
{
    public class facultyService
    {
        private readonly string _apiBaseUrl;
        apiService apiServices;

        public facultyService(Uri baseUrl)
        {
            _apiBaseUrl = baseUrl.ToString();
            apiServices = new apiService(_apiBaseUrl);
        }

        // Select faculties by ID
        public async Task<Tuple<int?, List<SelectListItem>>> getSelectedFaculties(int? selectedValue)
        {
            List<FacultyView> facultyList = await getfacultyAsync();
            List<SelectListItem> facultyOptions = facultyList
                .Select(faculty => new SelectListItem
                {
                    Value = faculty.id.ToString(),
                    Text = faculty.name,
                    Selected = selectedValue.HasValue && faculty.id == selectedValue.Value
                })
                .ToList();

            int? selectedCampusId = null;
            if (selectedValue.HasValue)
            {
                FacultyView selectedFaculty = facultyList.FirstOrDefault(f => f.id == selectedValue.Value);
                if (selectedFaculty != null)
                {
                    selectedCampusId = selectedFaculty.campus_id;
                }
            }

            return Tuple.Create(selectedCampusId, facultyOptions);
        }

        

        //Get Faculties of a Campus
        public async Task<List<Faculty>> getFacultybyCampusIdAsync(int campusId)
        {
            List<Faculty> listFaculty = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}Faculty/GetbyCampusId/{campusId}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listFaculty = JsonConvert.DeserializeObject<List<Faculty>>(data);
                }
            }

            return listFaculty;
        }

        //Get Faculties of a Campus
        public async Task<List<FacultyView>> getFacultybyCampusAsync(int campusId)
        {
            List<FacultyView> listFaculty = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}Faculty/GetbyCampus/{campusId}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listFaculty = JsonConvert.DeserializeObject<List<FacultyView>>(data);
                }
            }

            return listFaculty;
        }

        // Get all Faculities
        public async Task<List<FacultyView>> getfacultyAsync()
        {
            List<FacultyView> listFaculty = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}Faculty/Get"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listFaculty = JsonConvert.DeserializeObject<List<FacultyView>>(data);
                }
            }

            return listFaculty;
        }

    }
}