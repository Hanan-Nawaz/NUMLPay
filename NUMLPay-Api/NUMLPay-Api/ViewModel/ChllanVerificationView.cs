using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Humanizer.In;

namespace NUMLPay_Api.ViewModel
{
    public class ChllanVerificationView
    {
        public int challan_no { get; set; }
        public string Name { get; set; }
        public string std_numl_id { get; set; }
        public int semester { get; set; }
        public float total_fee { get; set; }
        public string due_date { get; set; }
        public string FeeFor { get; set; }
        public string degree_name { get; set; }
        public string fee_plan { get; set; }
        public string Session { get; set; }
        public string status { get; set; }
        public int fI_Id { get; set; }
        public int fee_type { get; set; }
    }
}