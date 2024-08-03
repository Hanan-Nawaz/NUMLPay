using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NUMLPay_WebApp.Models
{
    public class MiscellaneousFees
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description")]
        public int desc_id { get; set; }

        [ForeignKey("desc_id")]
        public Description description { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        public int amount { get; set; }

        [Required]
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