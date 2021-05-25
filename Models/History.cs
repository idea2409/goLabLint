using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace golablint.Models
{
    public class History {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        [ForeignKey("Equipment")]
        public Guid equipmentid {get; set;}
        [Required(ErrorMessage = "กรุณาระบุอุปกรณ์", AllowEmptyStrings = false)]
        public Equipment equipment { get; set; }

        [Required(ErrorMessage = "กรุณาระบุวันออกประวัติ", AllowEmptyStrings = false)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime issueDate { get; set; }

        [Required(ErrorMessage = "กรุณาระบุสถานะ", AllowEmptyStrings = false)]
        public string status {get; set; }
    
        [Required(ErrorMessage = "กรุณาระบุจำนวน",AllowEmptyStrings=false)]
        [Range(1,int.MaxValue,ErrorMessage=("กรุณาระบุจำนวนที่ถูกต้อง"))]
        public int amount { get; set; }
    }
}