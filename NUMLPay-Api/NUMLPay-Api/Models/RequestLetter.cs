using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NUMLPay_Api.Models
{
    public class RequestLetter
    {
        [Key]
        public int request_id { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        [Display(Name = "users")]
        public string std_numl_id { get; set; }

        [ForeignKey("std_numl_id")]
        public Users users { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(8000)]
        public string subject { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(8000)]
        public string body { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(8000)]
        public string supporting_material { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime date { get; set; }

        [Required]
        public int status { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(30)]
        [Display(Name = "Admin")]
        public string decision_by { get; set; }

        [ForeignKey("decision_by")]
        public Admin admin { get; set; }

    }
}