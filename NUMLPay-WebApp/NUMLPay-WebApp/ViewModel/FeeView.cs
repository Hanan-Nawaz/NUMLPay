using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUMLPay_WebApp.ViewModel
{
    public class FeeView
    {
        public int Id { get; set; }
        public string added_by { get; set; }
        public int total_fee { get; set; }
        public string fee_for { get; set; }
        public string academic_id { get; set; }
        public string Shift { get; set; }
        public string degree_name { get; set; }
        public string campus_name { get; set; }
        public string faculty_name { get; set; }
        public string dept_name { get; set; }
        public string session { get; set; }
    }

}