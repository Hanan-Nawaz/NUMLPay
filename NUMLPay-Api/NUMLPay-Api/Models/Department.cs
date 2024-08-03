using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace NUMLPay_Api.Models
{
    public class Department
    {
        [Key]
        public int id { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(200)]
        public string name { get; set; }

        [Required]
        [Display(Name = "Faculty")]
        public int faculty_id { get; set; }

        [ForeignKey("faculty_id")]
        public Faculty faculty { get; set; }

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