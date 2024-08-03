using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NUMLPay_WebApp.Models
{
    public class FeeStructure
    {
        [Key]
        public int Id { get; set; }

        public int? shift_id { get; set; }

        [Required(ErrorMessage = "Session is required.")]
        public int session { get; set; }

        [Required(ErrorMessage = "Fee For is required.")]
        public int fee_for { get; set; }

        [Required(ErrorMessage = "Total Fee is required.")]
        public int total_fee { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        [Display(Name = "Admin")]
        public string added_by { get; set; }

        [ForeignKey("added_by")]
        public Admin admin { get; set; }
    }
}