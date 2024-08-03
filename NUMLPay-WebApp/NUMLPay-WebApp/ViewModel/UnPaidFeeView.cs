using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUMLPay_WebApp.ViewModel
{
    public class UnPaidFeeView
    {
        public int challan_no { get; set; }
        public int semester { get; set; }
        public int fee_type { get; set; }
        public int fee_for { get; set; }
        public string FeeFor { get; set; }
        public string issue_date { get; set; }
        public string installment_no { get; set; }
        public float total_fee { get; set; }
        public string due_date { get; set; }
        public string status { get; set; }
        public string valid_date { get; set; }
        public int id { get; set; }
        public float fine { get; set; }
        public string paid_date { get; set; }
        public string payment_method { get; set; }

    }
}