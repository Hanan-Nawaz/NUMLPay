using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NUMLPay_Api.Models
{
    public class Challans
    {
        [Key]
        public int challan_id { get; set; }

        [Required]
        public int admissison_session { get; set; }

        [Required]
        public int session { get; set; }

        [Required]
        public string due_date { get; set; }

        [Required]
        public int fee_for { get; set; }

        [Required]
        public string valid_date { get; set; }

        [Display(Name = "Admin")]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        public string added_by { get; set; }

        [ForeignKey("added_by")]
        public Admin admin { get; set; }

        [Required]
        public int is_active { get; set; }

    }

}