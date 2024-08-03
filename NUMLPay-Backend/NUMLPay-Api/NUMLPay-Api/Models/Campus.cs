using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NUMLPay_Api.Models
{
    public class Campus
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string name { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(30)]
        public string added_by { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime date { get; set; }

        [Required]
        public int is_active { get; set; }
    }
}