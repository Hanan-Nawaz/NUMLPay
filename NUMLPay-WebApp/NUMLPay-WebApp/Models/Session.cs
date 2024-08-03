using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace NUMLPay_WebApp.Models
{
    public class Session
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "Session is required.")]
        public int name { get; set; }

        [Required(ErrorMessage = "Year is required.")]
        public int year { get; set; }

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