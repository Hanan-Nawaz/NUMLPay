using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NUMLPay_WebApp.Models
{
    public class Subjects
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string name { get; set; }

        [Display(Name = "Department")]
        public int? dept_id { get; set; }

        [ForeignKey("dept_id")]
        public Department department { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(30)]
        public string added_by { get; set; }

        [Required]
        public int is_active { get; set; }
    }
}