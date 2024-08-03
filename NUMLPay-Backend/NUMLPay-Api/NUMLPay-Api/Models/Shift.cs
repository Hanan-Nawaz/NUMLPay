using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NUMLPay_Api.Models
{
    public class Shift
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int academic_id { get; set; }

        [Required]
        public int shift { get; set; }

        [Display(Name = "Degree")]
        public int? degree_id { get; set; }

        [ForeignKey("degree_id")]
        public Degree degree { get; set; }

        [Required]
        public int is_active { get; set; }
    }
}