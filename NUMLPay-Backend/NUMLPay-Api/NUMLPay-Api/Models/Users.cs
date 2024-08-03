using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NUMLPay_Api.Models
{
    public class Users
    {
        [Key]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        public string numl_id {  get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(200)]
        public string name { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(1000)]
        public string password { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string father_name { get; set; }

        [Column(TypeName = "date")]
        public DateTime date_of_birth { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string email { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string contact { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(15)]
        public string nic { get; set; }

        [Display(Name = "Shift")]
        public int degree_id { get; set; }

        [ForeignKey("degree_id")]
        public Shift shift { get; set; }

        [Required]
        public int semester { get; set; }

        [Required]
        public int admission_session { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int dept_id { get; set; }

        [ForeignKey("dept_id")]
        public Department department { get; set; }

        [Display(Name = "FeePlan")]
        public int fee_plan { get; set; }

        [ForeignKey("fee_plan")]
        public FeePlan feePlan { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(8000)]
        public string image { get; set; }

        [Display(Name = "Admin")]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        public string verified_by { get; set; }

        [ForeignKey("verified_by")]
        public Admin admin { get; set; }

        public int status_of_degree { get; set; }

        public int is_relegated { get; set; }

        public int passed_ceased_sems { get; set; }

        public string fp_token { get; set; }

        public string fp_token_expiry { get; set; }

        [Required]
        public int is_active { get; set; }

    }
}