using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NUMLPay_Api.ViewModel
{
    public class DeptView
    {
        public int id { get; set; }
        public int faculty_id { get; set; }
        public string name { get; set; }
        public string added_by { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime date { get; set; }
        public string status { get; set; }
        public string faculty_name { get; set; }
        public string campus_name { get; set; }
    }
}