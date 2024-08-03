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
    public class subjectService
    {
        private readonly string _apiBaseUrl;
        apiService apiServices;

        public subjectService(Uri baseUrl)
        {
            _apiBaseUrl = baseUrl.ToString();
            apiServices = new apiService(_apiBaseUrl);
        }

        // Select subject by Id
        public async Task<List<SelectListItem>> getSelectedSubject(int? selectedValue)
        {
            List<Subjects> subjectList = new List<Subjects>();
           
                Subjects subjects = await getSubjectAsync(Convert.ToInt32(selectedValue));

            subjectList.Add(subjects);
            
            List<SelectListItem> subjectOptions = subjectList
                    .Select(subject => new SelectListItem
                    {
                        Value = subject.id.ToString(),
                        Text = subject.name
                    })
                    .ToList();

            if (selectedValue.HasValue)
            {
                foreach (var option in subjectOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return subjectOptions;
        }

        // Get only active Subjects
        public async Task<List<Subjects>> getActiveSubjectsAsync(int id)
        {
            List<Subjects> activeSubjectList = new List<Subjects>();
            List<Subjects> allSubjectList = await getSubjectsAsync(id);

            if (allSubjectList != null)
            {
                foreach (var subject in allSubjectList)
                {
                    if (subject.is_active == 1)
                    {
                        activeSubjectList.Add(subject);
                    }
                }
            }

            return activeSubjectList;
        }

        // Get all Subjects
        public async Task<List<Subjects>> getSubjectsAsync(int? id)
        {
            List<Subjects> listSubjects = null;
            if(id.HasValue)
            {
                using (var response = await apiServices.GetAsync($"{_apiBaseUrl}Subject/GetbyDeptId/{id}"))
                {
                    var content = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        listSubjects = JsonConvert.DeserializeObject<List<Subjects>>(data);
                    }
                }
            }

            return listSubjects;
        }

        // Get Subject by Id
        public async Task<Subjects> getSubjectAsync(int Id)
        {
            Subjects subject = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}/Subject/{Id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    subject = JsonConvert.DeserializeObject<Subjects>(data);
                }
            }

            return subject;
        }
    }
}
