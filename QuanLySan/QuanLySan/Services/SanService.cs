using QuanLySan.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLySan.Services
{
    internal class SanService
    {
        //Kiem tra khung gio hop le
        public static bool KiemTraKhungGioHopLe(TimeSpan gioBD, TimeSpan gioKT, ObservableCollection<GioSanItem> dsHienTai, out string msg)
        {
            if (gioBD >= gioKT)
            {
                msg = "Giờ bắt đầu phải nhỏ hơn giờ kết thúc.";
                return false;
            }
            foreach (var item in dsHienTai)
            {
                if (TimeSpan.TryParse(item.GioBatDau, out TimeSpan itemBD) &&
                    TimeSpan.TryParse(item.GioKetThuc, out TimeSpan itemKT))
                {
                    if (gioBD < itemKT && itemBD < gioKT)
                    {
                        msg = $"Khung giờ bị chồng lấn với khung giờ đã có: {item.GioBatDau} - {item.GioKetThuc}";
                        return false;
                    }
                }
            }
            msg = "Hợp lệ";
            return true;
        }
    }
}
