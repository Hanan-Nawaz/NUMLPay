using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUMLPay_Api.ViewModel
{
    public class ReportFee
    {
        public string numl_id { get; set; }
        public string name { get; set; }
        public string semester { get; set; }
        public string degree_name { get; set; }
        public double total_fee { get; set; }
        public string fee_status { get; set; }
    }
}
