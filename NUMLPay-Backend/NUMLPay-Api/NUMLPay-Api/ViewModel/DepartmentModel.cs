using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUMLPay_Api.ViewModel
{
    public class DepartmentModel
    {
        public int DeptId { get; set; }
        public string DeptName { get; set; }
        public int FacultyId { get; set; }
        public int TotalStudents { get; set; }
    }

}