using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace NUMLPay_WebApp.Models
{
    public class Faculty
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        public string name { get; set; }

        [Required(ErrorMessage = "Campus is required.")]
        [Display(Name = "Campus")]
        public int campus_id { get; set; }

        [ForeignKey("campus_id")]
        public Campus campus { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        public string added_by { get; set; }

        [Required]
        public DateTime date { get; set; }

        [Required]
        public int is_active { get; set; }
    }
}