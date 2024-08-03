using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUMLPay_Api.CreateModel
{
    public class CreateFeeAndAccountBook
    {
        public int Id { get; set; }
        public string numlId { get; set; }
        public int status { get; set; }
        public string verified_by { get; set; }
    }
}