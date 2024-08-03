using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUMLPay_Api.ViewModel
{
    public class GetFeesForGeneration
    {
        public int challan_id { get; set; }
        public int Id { get; set; }
        public string due_date { get; set; }
        public int mode { get; set; }
        public string valid_date { get; set; }
        public int discount { get; set; }
        public int tuition_fee { get; set; }
        public double total_fee { get; set; }
        public int security { get; set; }
    }
}