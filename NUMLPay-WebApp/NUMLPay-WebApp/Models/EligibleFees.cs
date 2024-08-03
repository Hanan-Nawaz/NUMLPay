using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NUMLPay_WebApp.Models
{
    public class EligibleFees
    {
        [Key]
        public int id { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        [Display(Name = "users")]
        public string std_numl_id { get; set; }


        [ForeignKey("std_numl_id")]
        public Users users { get; set; }


        [Required]
        public int semester_fee { get; set; }

        [Required]
        public int hostel_fee { get; set; }

        [Required]
        public int bus_fee { get; set;}
    }
}