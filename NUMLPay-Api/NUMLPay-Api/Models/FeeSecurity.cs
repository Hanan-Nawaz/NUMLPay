using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace NUMLPay_Api.Models
{
    public class FeeSecurity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "FeeStructure")]
        public int fee_structure_id { get; set; }

        [ForeignKey("fee_structure_id")]
        public FeeStructure feeStructure { get; set; }

        [Required]
        public int security { get; set; }
    }
}