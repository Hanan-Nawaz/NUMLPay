using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NUMLPay_WebApp.Models
{
    public class Users
    {
        [Key]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        [Required(ErrorMessage = "NUML ID is required.")]
        [RegularExpression("^NUML.*", ErrorMessage = "NUML Id must include 'NUML' in start")]
        public string numl_id {  get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string name { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [Column(TypeName = "varchar")]
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*[!@#$%^&*()_+=\\-[\\]{};':\"|,.<>/?]).{8,}$", ErrorMessage = "Password must be 8 Characters Long and must Include atleast one Uppercase, Lowecase and Special Character.")]
        [StringLength(1000, MinimumLength = 8, ErrorMessage = "Password must be of 8 Character.")]
        public string password { get; set; }

        [Required(ErrorMessage = "Father Name is required.")]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string father_name { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        [Column(TypeName = "date")]
        public DateTime date_of_birth { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string email { get; set; }

        [Required(ErrorMessage = "Contact Number is required.")]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string contact { get; set; }

        [Required(ErrorMessage = "CNIC is required.")]
        [Column(TypeName = "varchar")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "CNIC consists of Numbers (0-9).")]
        [StringLength(15, MinimumLength = 13, ErrorMessage = "CNIC must be of 13 Character.")]
        public string nic { get; set; }

        [Required(ErrorMessage = "Degree is required.")]
        [Display(Name = "Shift")]
        public int degree_id { get; set; }

        [ForeignKey("degree_id")]
        public Shift shift { get; set; }

        [Required(ErrorMessage = "Semester is required.")]
        public int semester { get; set; }

        [Required(ErrorMessage = "Admission Session is required.")]
        public int admission_session { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [Display(Name = "Department")]
        public int dept_id { get; set; }

        [ForeignKey("dept_id")]
        public Department department { get; set; }

        [Required(ErrorMessage = "Fee Plan is required.")]
        [Display(Name = "FeePlan")]
        public int fee_plan { get; set; }

        [ForeignKey("fee_plan")]
        public FeePlan feePlan { get; set; }

        [Required(ErrorMessage = "Image is required.")]
        [Column(TypeName = "varchar")]
        [StringLength(8000)]
        public string image { get; set; }

        [Display(Name = "Admin")]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        public string verified_by { get; set; }

        [ForeignKey("verified_by")]
        public Admin admin { get; set; }

        [Required(ErrorMessage = "Status of Degree is required.")]
        public int status_of_degree { get; set; }

        public int is_relegated { get; set; }

        public int passed_ceased_sems { get; set; }

        public string fp_token { get; set; }

        public string fp_token_expiry { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public int is_active { get; set; }

    }
}