using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace golablint.Models {
    public class Borrowing {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        [Required(ErrorMessage = "กรุณาระบุอุปกรณ์ที่จะยืม", AllowEmptyStrings = false)]
        public Equipment equipment { get; set; }

        [Required(ErrorMessage = "กรุณาระบุผู้ใช้ที่จะยืม", AllowEmptyStrings = false)]
        public User user { get; set; }

        [Required(ErrorMessage = "กรุณาระบุวันเริ่มยืม", AllowEmptyStrings = false)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime startDate { get; set; }

        [Required(ErrorMessage = "กรุณาระบุวันคืน", AllowEmptyStrings = false)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime endDate { get; set; }

        [Required(ErrorMessage = "กรุณาระบุสถานะ", AllowEmptyStrings = false)]
        public string status {get; set; }

    }
}