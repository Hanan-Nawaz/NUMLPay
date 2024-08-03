using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUMLPay_WebApp.ViewModel
{
    public class SummerEnrollmentView
    {
        public int id { get; set; }
        public string numl_id { get; set; }

        public string subject_name { get; set; }

        public int session_year { get; set; }

        public string added_by { get; set; }
    }
}