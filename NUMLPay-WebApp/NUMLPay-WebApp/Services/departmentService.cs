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
    public class departmentService
    {
        private readonly string _apiBaseUrl;
        apiService apiServices;

        public departmentService(Uri baseUrl)
        {
            _apiBaseUrl = baseUrl.ToString();
            apiServices = new apiService(_apiBaseUrl);
        }

        //Get Depts of a Faculty
        public async Task<List<Department>> getDepartmentAsync(int facultyId)
        {
            List<Department> listDept = null;
            using (var response = await apiServices.GetAsync($"{_apiBaseUrl}Department/GetbyFacultyId/{facultyId}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listDept = JsonConvert.DeserializeObject<List<Department>>(data);
                }
            }

            return listDept;
        }

        // Get all Departments
        public async Task<List<DeptView>> getDepartmentAsync(int? campusId)
        {
            List<DeptView> listDepartments = null;
            if (campusId.HasValue)
            {
                using (var response = await apiServices.GetAsync($"{_apiBaseUrl}Department/GetbyCampusId/{campusId}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        listDepartments = JsonConvert.DeserializeObject<List<DeptView>>(data);
                    }
                }
            }
            else
            {
                using (var response = await apiServices.GetAsync($"{_apiBaseUrl}Department/Get"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        listDepartments = JsonConvert.DeserializeObject<List<DeptView>>(data);
                    }
                }
            }
           

            return listDepartments;
        }

        //select camous by Id
        public async Task<List<SelectListItem>> getSelectedDept(int? selectedValue)
        {
            List<DeptView> deptList = await getDepartmentAsync(null);
            List<SelectListItem> deptOptions = deptList
                    .Select(dept => new SelectListItem
                    {
                        Value = dept.id.ToString(),
                        Text = dept.name,
                    })
                    .ToList();

            if (selectedValue.HasValue)
            {
                foreach (var option in deptOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return deptOptions;
        }

        // Select departments by ID
        public async Task<Tuple<int?, List<SelectListItem>>> getSelectedDepartments(int? selectedValue)
        {
            List<DeptView> departmentList = await getDepartmentAsync(null);
            List<SelectListItem> departmentOptions = departmentList
                .Select(department => new SelectListItem
                {
                    Value = department.id.ToString(),
                    Text = department.name,
                    Selected = selectedValue.HasValue && department.id == selectedValue.Value
                })
                .ToList();

            int? selectedFacultyId = null;
            if (selectedValue.HasValue)
            {
                DeptView selectedDepartment = departmentList.FirstOrDefault(d => d.id == selectedValue.Value);
                selectedFacultyId = selectedDepartment.faculty_id;
            }

            return Tuple.Create(selectedFacultyId, departmentOptions);
        }

    }
}