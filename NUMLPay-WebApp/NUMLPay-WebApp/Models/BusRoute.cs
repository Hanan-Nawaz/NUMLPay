using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NUMLPay_WebApp.Models
{
    public class BusRoute
    {
        [Key]
        public int id { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        public string name { get; set; }

        [Required]
        [Display(Name = "Campus")]
        public int campus_id { get; set; }

        [ForeignKey("campus_id")]
        public Campus campus { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        public string added_by { get; set; }

        [Required]
        public int is_active { get; set; }
    }
}