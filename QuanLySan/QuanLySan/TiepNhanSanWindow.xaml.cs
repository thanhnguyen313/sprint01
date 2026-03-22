#nullable enable
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace QuanLySan
{
    /// <summary>
    /// Model đại diện cho 1 dòng giờ sân trong DataGrid
    /// </summary>
    public class GioSanItem : INotifyPropertyChanged
    {
        private int _stt;
        private string _gioBatDau = "";
        private string _gioKetThuc = "";
        private decimal _donGia;

        public int STT
        {
            get => _stt;
            set { _stt = value; OnPropertyChanged(); }
        }

        public string GioBatDau
        {
            get => _gioBatDau;
            set { _gioBatDau = value; OnPropertyChanged(); }
        }

        public string GioKetThuc
        {
            get => _gioKetThuc;
            set { _gioKetThuc = value; OnPropertyChanged(); }
        }

        public decimal DonGia
        {
            get => _donGia;
            set { _donGia = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public partial class TiepNhanSanWindow : Window
    {
        // Danh sách giờ sân – bind vào DataGrid
        private ObservableCollection<GioSanItem> _dsGioSan = new();

        public TiepNhanSanWindow()
        {
            InitializeComponent();

            // Load dữ liệu ComboBox
            cboMaLoaiSan.ItemsSource = new[] { "Sân bóng đá", "Sân cầu lông", "Sân pickleball" };
            cboTinhTrang.ItemsSource = new[] { "Hoạt động", "Bảo trì" };

            dgGioSan.ItemsSource = _dsGioSan;
        }

        // ── Thêm sân mới (nút "Thêm") ──
        private void BtnThemSan_Click(object sender, RoutedEventArgs e)
        {
            // Validate bắt buộc
            if (string.IsNullOrWhiteSpace(txtTenSan.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sân.", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTenSan.Focus();
                return;
            }

            if (cboMaLoaiSan.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn loại sân.", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDiaChi.Text))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ.", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtDiaChi.Focus();
                return;
            }

            // Phát sinh mã sân (tạm thời dùng GUID ngắn)
            string maSan = "SAN" + DateTime.Now.ToString("yyyyMMddHHmmss");
            txtMaSan.Text = maSan;

            MessageBox.Show($"Đã thêm sân \"{txtTenSan.Text}\" thành công!\nMã sân: {maSan}",
                "Thêm sân", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ── Hủy / Xóa trắng form ──
        private void BtnHuy_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có muốn xóa trắng toàn bộ dữ liệu đã nhập?",
                "Xác nhận hủy", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                ClearForm();
            }
        }

        // ── Thoát / Đóng cửa sổ ──
        private void BtnThoat_Click(object sender, RoutedEventArgs e) => this.Close();

        // ── Thêm 1 dòng trống vào grid để user nhập ──
        private void BtnThemGio_Click(object sender, RoutedEventArgs e)
        {
            _dsGioSan.Add(new GioSanItem
            {
                STT = _dsGioSan.Count + 1,
                GioBatDau = "",
                GioKetThuc = "",
                DonGia = 0
            });

            // Focus vào dòng mới
            dgGioSan.ScrollIntoView(_dsGioSan[_dsGioSan.Count - 1]);
            dgGioSan.SelectedIndex = _dsGioSan.Count - 1;
        }

        // ── Xóa dòng đang chọn ──
        private void BtnXoaGio_Click(object sender, RoutedEventArgs e)
        {
            if (dgGioSan.SelectedItem is GioSanItem dongChon)
            {
                _dsGioSan.Remove(dongChon);
                // Cập nhật lại STT
                for (int i = 0; i < _dsGioSan.Count; i++)
                {
                    _dsGioSan[i].STT = i + 1;
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng trong bảng để xóa.",
                    "Chưa chọn dòng", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ── Cho phép edit tất cả cột trừ STT ──
        private void DgGioSan_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            // STT là cột index 0 – không cho sửa (đã set IsReadOnly trong XAML)
        }

        // ── Validate khi user rời ô edit ──
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
                    if (!string.IsNullOrEmpty(newValue) && !TimeSpan.TryParse(newValue, out _))
                    {
                        MessageBox.Show("Vui lòng nhập giờ theo định dạng HH:mm (VD: 07:00)",
                            "Sai định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                        e.Cancel = true;
                        return;
                    }
                }

                // Validate đơn giá
                if (columnHeader == "Đơn giá")
                {
                    // Loại bỏ dấu phân cách hàng nghìn để parse
                    string cleaned = newValue.Replace(",", "").Replace(".", "");
                    if (!string.IsNullOrEmpty(cleaned) && !decimal.TryParse(cleaned, out _))
                    {
                        MessageBox.Show("Đơn giá phải là số.",
                            "Sai định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                        e.Cancel = true;
                        return;
                    }
                }
            }
        }

        // ── Lưu (placeholder) ──
        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng lưu đang được phát triển.",
                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ── Di chuyển cửa sổ khi nắm thanh Header ──
        private void Header_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        // ── Helper: Xóa trắng toàn bộ form ──
        private void ClearForm()
        {
            txtMaSan.Text = "";
            txtTenSan.Text = "";
            cboMaLoaiSan.SelectedIndex = -1;
            txtDiaChi.Text = "";
            txtGhiChu.Text = "";
            cboTinhTrang.SelectedIndex = -1;
            _dsGioSan.Clear();
        }
    }
}
