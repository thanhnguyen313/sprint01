using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLySan.Views
{
    public partial class TiepNhanSanWindow : Window
    {
        public TiepNhanSanWindow()
        {
            InitializeComponent();
            // Thiết lập DataContext để liên kết dữ liệu với ViewModel
            this.DataContext = new QuanLySan.ViewModels.TiepNhanSanViewModel();
        }

        // Xử lý di chuyển cửa sổ khi nhấn giữ Header (UI-only logic, thuộc View)
        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        // Thoát chương trình (UI-only logic, thuộc View)
        private void BtnThoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Validate khi user rời ô edit trong DataGrid (UI validation, thuộc View)
        private void DgGioSan_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel)
                return;

            if (e.EditingElement is TextBox tb)
            {
                string columnHeader = (e.Column.Header ?? "").ToString()!;
                string newValue = tb.Text.Trim();

                // Validate giờ bắt đầu / kết thúc
                if (columnHeader == "Giờ bắt đầu" || columnHeader == "Giờ kết thúc")
                {
                    if (!string.IsNullOrEmpty(newValue) && !System.TimeSpan.TryParse(newValue, out _))
                    {
                        MessageBox.Show("Vui lòng nhập giờ theo định dạng HH:mm (VD: 07:00)",
                            "Sai định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                        e.Cancel = true;
                        return;
                    }
                }
            }
        }
    }
}
