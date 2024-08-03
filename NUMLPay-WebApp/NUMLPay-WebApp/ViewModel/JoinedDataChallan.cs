using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUMLPay_WebApp.ModelFront
{
    public class JoinedDataChallan
    {
        public int challan_no { get; set; }
        public string issue_date { get; set; }
        public int installment_no { get; set; }
        public float fine { get; set; }
        public float total_fee { get; set; }
        public string valid_date { get; set; }
        public string due_date { get; set; }
        public string paid_date { get; set; }
        public string Session { get; set; }
        public int currentSem { get; set; }
        public int feeSem { get; set; }
        public string numl_id { get; set; }
        public string name { get; set; }
        public string father_name { get; set; }
        public string fee_plan { get; set; }
        public string degree_name { get; set; }
        public int fee_id { get; set; }
        public int? fee_for { get; set; }
        public string FeeFor { get; set; }
        public string image { get; set; }
        public string email { get; set; }

    }
}