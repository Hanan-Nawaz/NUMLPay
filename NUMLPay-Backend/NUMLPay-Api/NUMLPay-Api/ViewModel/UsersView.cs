using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUMLPay_Api.ViewModel
{
    public class UsersView
    {
        public string numl_id { get; set; }
        public string name { get; set; }
        public string dept_name { get; set; }
        public string faculty_name { get; set; }
        public string campus_name { get; set; }
        public int semester { get; set; }
        public string status_of_degree { get; set; }
        public string status { get; set; }
        public string shift { get; set; }
        public string academic_id { get; set; }
        public string degree_name { get; set; }
    }

}