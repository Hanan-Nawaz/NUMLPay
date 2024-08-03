using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NUMLPay_Api.Models
{
    public class SummerFee
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "Fee is required.")]
        public int fee { get; set; }

        [Required]
        [Display(Name = "subjects")]
        public int subject_id { get; set; }

        [ForeignKey("subject_id")]
        public Subjects subjects { get; set; }

        [Required(ErrorMessage = "Seesion Year is required.")]
        public int session_year { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(30)]
        public string added_by { get; set; }

        [Required]
        public int is_active { get; set; }
    }
}