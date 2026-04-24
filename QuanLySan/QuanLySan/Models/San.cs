using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLySan.Models
{
    public class San
    {
        public string MaSan { get; set; } = string.Empty;
        public string TenSan { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public string MaLoaiSan { get; set; } = string.Empty; // Khóa ngoại từ LoaiSan
        public string MaTinhTrang { get; set; } = string.Empty; // Khóa ngoại từ TinhTrang
        public string GhiChu { get; set; } = string.Empty;
    }
}
