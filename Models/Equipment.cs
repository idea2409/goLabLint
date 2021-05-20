using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace golablint.Models
{
    public class Equipment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        
        [Required(ErrorMessage = "กรุณาชื่ออุปกรณ์",AllowEmptyStrings=false)]
        public string name { get; set; }

        public string description { get; set; }

        [Required(ErrorMessage = "กรุณาใส่รูปภาพ",AllowEmptyStrings=false)]
        public byte[] image {get;set;}
                
        [Required(ErrorMessage = "กรุณาระบุจำนวน",AllowEmptyStrings=false)]
        [Range(0,int.MaxValue,ErrorMessage=("กรุณาระบุจำนวนที่ถูกต้อง"))]
        public int amount { get; set; }
    }
}