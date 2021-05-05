using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace golablint.Models {
    public class User {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string name { get; set; }

        [Required(ErrorMessage = "Surname is required.")]
        public string surname { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email is invalid.")]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password length must be greater than 8.")]
        [Number(ErrorMessage = "Password must contain digits 0-9")]
        [LowerCase(ErrorMessage = "Password must contain at least one or more lowercase character a-z")]
        [UpperCase(ErrorMessage = "Password must contain at least one or more uppercase character A-Z")]
        public string password { get; set; }

        public string role { get; set; }
    }
}

public class Number : RegularExpressionAttribute {
    public Number() : base(".*\\d+.*") { }
}

public class LowerCase : RegularExpressionAttribute {
    public LowerCase() : base(".*[a-z]+.*") { }
}
public class UpperCase : RegularExpressionAttribute {
    public UpperCase() : base(".*[A-Z]+.*") { }
}