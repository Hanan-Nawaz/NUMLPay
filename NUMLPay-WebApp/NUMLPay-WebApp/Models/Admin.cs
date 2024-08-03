using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Models
{
    public class Admin
    {
        [Key]
        [Required(ErrorMessage = "Email Id is required.")]
        [RegularExpression(@"^[a-zA-Z0-9_\\.-]+@numl\.edu\.pk$", ErrorMessage = "Email is not valid. Must include @numl.edu.pk.")]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        public string email_id { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [Column(TypeName = "varchar")]
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*[!@#$%^&*()_+=\\-[\\]{};':\"|,.<>/?]).{8,}$", ErrorMessage = "Password must be 8 Characters Long and must Include atleast one Uppercase, Lowecase and Special Character.")]
        [StringLength(1000, MinimumLength = 8, ErrorMessage = "Password must be of 8 Character.")]
        public string password { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [Column(TypeName = "varchar")]
        [StringLength(25)]
        public string name { get; set; }

        [Required(ErrorMessage = "Post is required.")]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        public string post { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public int role { get; set; }

        [Display(Name = "Campus")]
        [RequiredIf("role","1" ,"3", ErrorMessage = "Campus is required.")]
        public int? campus_id { get; set; }

        [ForeignKey("campus_id")]
        public Campus campus { get; set; }

        [Display(Name = "Faculty")]
        [RequiredIf("role", "3", ErrorMessage = "Faculty is required.")]
        public int? faculty_id { get; set; }

        [ForeignKey("faculty_id")]
        public Faculty faculty { get; set; }

        [Display(Name = "Department")]
        [RequiredIf("role", "3", ErrorMessage = "Department is required.")]
        public int? dept_id { get; set; }

        [ForeignKey("dept_id")]
        public Department department { get; set; }


        [Column(TypeName = "varchar")]
        [StringLength(30)]
        public string added_by { get; set; }

        public string fp_token { get; set; }

        public DateTime fp_token_expiry { get; set; }

        public int is_active { get; set; }

    }

    public class RequiredIfAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string _dependentPropertyName;
        private readonly string[] _targetValues;

        public RequiredIfAttribute(string dependentPropertyName, params string[] targetValues)
        {
            _dependentPropertyName = dependentPropertyName;
            _targetValues = targetValues;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dependentProperty = validationContext.ObjectType.GetProperty(_dependentPropertyName);
            var dependentValue = dependentProperty.GetValue(validationContext.ObjectInstance, null);

            if (dependentValue != null && _targetValues.Contains(dependentValue.ToString()) && value == null)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "requiredif",
            };
            rule.ValidationParameters.Add("dependentproperty", _dependentPropertyName);
            rule.ValidationParameters.Add("targetvalues", string.Join(",", _targetValues));
            yield return rule;
        }
    }
}
