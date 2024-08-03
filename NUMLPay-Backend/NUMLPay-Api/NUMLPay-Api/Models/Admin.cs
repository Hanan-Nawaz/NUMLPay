using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NUMLPay_Api.Models
{
    public class Admin
    {
        [Key]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        public string email_id { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(1000)]
        public string password { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(25)]
        public string name { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        public string post { get; set; }

        [Display(Name = "Campus")]
        public int? campus_id { get; set; }

        [ForeignKey("campus_id")]
        public Campus campus { get; set; }

        [Display(Name = "Faculty")]
        public int? faculty_id { get; set; }

        [ForeignKey("faculty_id")]
        public Faculty faculty { get; set; }

        [Display(Name = "Department")]
        public int? dept_id { get; set; }

        [ForeignKey("dept_id")]
        public Department department { get; set; }

        [Required]
        public int role { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        public string added_by { get; set; }

        public string fp_token { get; set; }

        public DateTime fp_token_expiry { get; set; }

        [Required]
        public int is_active { get; set; }

    }
}