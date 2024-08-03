using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NUMLPay_Api.ViewModel
{
    public class BusRouteView
    {
        public int id { get; set; }
        public int campus_id { get; set; }
        public string name { get; set; }
        public string added_by { get; set; }
        public int status { get; set; }
        public string campus_name { get; set; }
    }
}