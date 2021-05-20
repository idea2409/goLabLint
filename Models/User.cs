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

        [Required(ErrorMessage = "กรุณากรอกชื่อจริง", AllowEmptyStrings = false)]
        public string name { get; set; }

        [Required(ErrorMessage = "กรุณากรอกนามสกุล", AllowEmptyStrings = false)]
        public string surname { get; set; }

        [Required(ErrorMessage = "กรุณากรอกอีเมล", AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "อีเมลไม่ถูกต้อง")]
        public string email { get; set; }

        [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน", AllowEmptyStrings = false)]
        [MinLength(8, ErrorMessage = "รหัสผ่านต้องมีความยาวอย่างน้อย 8 อักขระ")]
        [Number(ErrorMessage = "รหัสผ่านต้องมีตัวเลข 0-9")]
        [LowerCase(ErrorMessage = "รหัสผ่านต้องมีตัวอักษรพิมพ์เล็ก a-z")]
        [UpperCase(ErrorMessage = "รหัสผ่านต้องมีตัวอักษรพิมพ์ใหญ่ A-Z")]
        public string password { get; set; }

        [Required]
        public string role { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "กรุณายืนยันรหัสผ่าน", AllowEmptyStrings = false)]
        [Compare("password", ErrorMessage = "รหัสผ่านที่ยืนยันไม่ตรงกับรหัสผ่านข้างต้น")]
        public string confirmPassword { get; set; }
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