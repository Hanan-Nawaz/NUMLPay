using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NUMLPay_WebApp.Models
{
    public class Degree
    {
        [Key]
        public int id { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string name { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int dept_id { get; set; }

        [ForeignKey("dept_id")]
        public Department department { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        [Display(Name = "Admin")]
        public string added_by { get; set; }

        [ForeignKey("added_by")]
        public Admin admin { get; set; }

        [Required]
        public DateTime date { get; set; }

      

    }
}