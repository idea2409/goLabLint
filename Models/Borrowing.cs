using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using FoolProof.Core;

namespace golablint.Models
{
    public class Borrowing
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        
        [Required]
        public Equipment equipment { get; set; }

        [Required]
        public User user { get; set; }

        [Required]
        public DateTime startDate {get; set;}

        [Required]   
        [GreaterThan("startDate")]
        public DateTime endDate {get;set;}
    }
}