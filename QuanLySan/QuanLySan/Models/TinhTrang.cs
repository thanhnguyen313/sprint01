using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLySan.Models
{
    public class TinhTrang
    {
        public string MaTinhTrang { get; set; } = string.Empty; // Khóa chính
        public string TenTinhTrang { get; set; } = string.Empty; // VD: Hoạt động
    }
}
