using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NUMLPay_Api.ViewModel
{
    public class SummerFeeView
    {
        public int id { get; set; }

        public int fee { get; set; }

        public int session_year { get; set; }

        public string subject_name { get; set; }
        
        public string added_by { get; set; }

        public int is_active { get; set; }
    }
}