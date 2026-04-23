using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuanLySan.ViewModels;

namespace QuanLySan.Views
{
    public partial class TiepNhanSanWindow : Window
    {
        public TiepNhanSanWindow()
        {
            InitializeComponent();
            // Thiết lập DataContext để liên kết dữ liệu với ViewModel
            this.DataContext = new TiepNhanSanViewModel();
        }

        // Xử lý di chuyển cửa sổ khi nhấn giữ Header (Dòng 12 trong XAML)
        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        // Thoát chương trình (Dòng 16 và 71 trong XAML)
        private void BtnThoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Xử lý khi kết thúc chỉnh sửa ô trong DataGrid (Dòng 40 trong XAML)
        private void DgGioSan_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Có thể thêm logic kiểm tra dữ liệu tại đây nếu cần
        }

        // Nút Thêm sân (Dòng 66 trong XAML)
        private void BtnThemSan_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is TiepNhanSanViewModel vm)
            {
                vm.ThemGioCommand.Execute(null);
            }
        }

        // Nút Lưu thông tin (Dòng 68 trong XAML)
        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is TiepNhanSanViewModel vm)
            {
                vm.LuuCommand.Execute(null);
            }
        }

        // Nút Hủy bỏ (Dòng 70 trong XAML)
        private void BtnHuy_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is TiepNhanSanViewModel vm)
            {
                // Gọi lệnh Hủy từ ViewModel
                vm.HuyCommand.Execute(null);
            }
        }
    }
}