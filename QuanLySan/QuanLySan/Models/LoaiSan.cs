using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLySan.Models
{
    public class LoaiSan
    {
        public string MaLoaiSan { get; set; } = string.Empty; // Khóa chính
        public string TenLoaiSan { get; set; } = string.Empty; // VD: Sân bóng đá
    }
}
