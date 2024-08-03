using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NUMLPay_WebApp.Models
{
    public class SubFeeStructure
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "FeeStructure")]
        public int fee_structure_id { get; set; }

        [ForeignKey("fee_structure_id")]
        public FeeStructure feeStructure { get; set; }

        [Required]
        public int audio_it { get; set; }

        [Required]
        public int maintainence { get; set; }

        [Required]
        public int computer_lab { get; set; }

        [Required]
        public int tutition_fee { get; set; }

        [Required]
        public int library { get; set; }

        [Required]
        public int exam_fee { get; set; }

        [Required]
        public int sports { get; set; }

        [Required]
        public int medical { get; set; }

        [Required]
        public int magazine { get; set; }

        [Required]
        public int admission_fee { get; set; }

        [Required]
        public int library_security { get; set; }

        [Required]
        public int registration_fee { get; set; }
    }
}