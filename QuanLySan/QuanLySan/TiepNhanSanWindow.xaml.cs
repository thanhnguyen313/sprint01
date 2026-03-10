using System.Windows;

namespace QuanLySan
{
    public partial class TiepNhanSanWindow : Window
    {
        public TiepNhanSanWindow()
        {
            InitializeComponent();
        }

        // Các event handler rỗng – chỉ để XAML build được
        private void BtnClose_Click(object sender, RoutedEventArgs e) { }
        private void BtnXoaDongGia_Click(object sender, RoutedEventArgs e) { }
        private void BtnLuu_Click(object sender, RoutedEventArgs e) { }
        private void BtnLamMoi_Click(object sender, RoutedEventArgs e) { }
    }
}
