using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace QuanLySan
{
    /// <summary>
    /// Model đại diện cho 1 dòng trong bảng giá (thực thể DONGIA)
    /// </summary>
    public class DonGiaItem
    {
        public int STT { get; set; }
        public string GioBatDau { get; set; } = "";
        public string GioKetThuc { get; set; } = "";
        public string LoaiNgay { get; set; } = "";
        public decimal DonGia { get; set; }
    }

    public partial class TiepNhanSanWindow : Window
    {
        // Danh sách dòng giá – bind vào DataGrid
        private ObservableCollection<DonGiaItem> _dsBangGia = new();

        // Quy định 1: Đơn giá theo loại ngày
        private readonly Dictionary<string, decimal> _bangGiaTheoLoaiNgay = new()
        {
            { "Thường",    50000 },
            { "Cuối tuần", 70000 },
            { "Lễ",       100000 }
        };

        public TiepNhanSanWindow()
        {
            InitializeComponent();

            // Load dữ liệu ComboBox theo Quy định 1
            cboLoaiSan.ItemsSource = new[] { "Sân bóng đá", "Sân cầu lông", "Sân pickleball" };
            cboTinhTrang.ItemsSource = new[] { "Hoạt động", "Bảo trì" };
            cboLoaiNgay.ItemsSource = new[] { "Thường", "Cuối tuần", "Lễ" };

            dgBangGia.ItemsSource = _dsBangGia;
        }

        // ── Khi chọn Loại ngày → Đơn giá tự nhảy ──
        private void CboLoaiNgay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboLoaiNgay.SelectedItem is string loaiNgay &&
                _bangGiaTheoLoaiNgay.TryGetValue(loaiNgay, out decimal gia))
            {
                txtDonGia.Text = gia.ToString("N0");
            }
            else
            {
                txtDonGia.Text = "";
            }
        }

        // ── Thêm dòng giá vào bảng ──
        private void BtnThemGia_Click(object sender, RoutedEventArgs e)
        {
            // Validate: không để trống
            if (string.IsNullOrWhiteSpace(txtGioBatDau.Text) ||
                string.IsNullOrWhiteSpace(txtGioKetThuc.Text) ||
                cboLoaiNgay.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Giờ bắt đầu, Giờ kết thúc và Loại ngày.",
                    "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Parse giờ
            if (!TimeSpan.TryParse(txtGioBatDau.Text.Trim(), out TimeSpan gioBD) ||
                !TimeSpan.TryParse(txtGioKetThuc.Text.Trim(), out TimeSpan gioKT))
            {
                MessageBox.Show("Giờ bắt đầu / kết thúc không hợp lệ.\nVui lòng nhập theo định dạng HH:mm (VD: 07:00)",
                    "Sai định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate: giờ bắt đầu < giờ kết thúc
            if (gioBD >= gioKT)
            {
                MessageBox.Show("Giờ bắt đầu phải nhỏ hơn Giờ kết thúc.",
                    "Sai khung giờ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate: khung giờ không chồng lấn với các dòng đã có
            foreach (var dong in _dsBangGia)
            {
                if (TimeSpan.TryParse(dong.GioBatDau, out TimeSpan bdCu) &&
                    TimeSpan.TryParse(dong.GioKetThuc, out TimeSpan ktCu))
                {
                    // Chồng lấn khi: gioBD < ktCu AND gioKT > bdCu
                    if (gioBD < ktCu && gioKT > bdCu)
                    {
                        MessageBox.Show($"Khung giờ {txtGioBatDau.Text} - {txtGioKetThuc.Text} bị chồng lấn với dòng {dong.STT} ({dong.GioBatDau} - {dong.GioKetThuc}).",
                            "Khung giờ chồng lấn", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }

            // Lấy đơn giá từ quy định
            string loaiNgay = cboLoaiNgay.SelectedItem.ToString()!;
            decimal donGia = _bangGiaTheoLoaiNgay[loaiNgay];

            // Thêm dòng mới
            _dsBangGia.Add(new DonGiaItem
            {
                STT = _dsBangGia.Count + 1,
                GioBatDau = txtGioBatDau.Text.Trim(),
                GioKetThuc = txtGioKetThuc.Text.Trim(),
                LoaiNgay = loaiNgay,
                DonGia = donGia
            });

            // Reset ô nhập để thêm dòng tiếp
            txtGioBatDau.Text = "";
            txtGioKetThuc.Text = "";
            cboLoaiNgay.SelectedIndex = -1;
            txtDonGia.Text = "";
        }

        // ── Xóa dòng đang chọn trong bảng ──
        private void BtnXoaDongGia_Click(object sender, RoutedEventArgs e)
        {
            if (dgBangGia.SelectedItem is DonGiaItem dongChon)
            {
                _dsBangGia.Remove(dongChon);
                // Cập nhật lại STT
                for (int i = 0; i < _dsBangGia.Count; i++)
                {
                    _dsBangGia[i].STT = i + 1;
                }
                dgBangGia.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng trong bảng để xóa.",
                    "Chưa chọn dòng", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ── Đóng cửa sổ ──
        private void BtnClose_Click(object sender, RoutedEventArgs e) => this.Close();

        // ── Di chuyển cửa sổ khi nắm thanh Header ──
        private void Header_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        // Placeholder
        private void BtnLuu_Click(object sender, RoutedEventArgs e) {
            // Bước 1: Kiểm tra tính đầy đủ của thông tin Sân
            if (string.IsNullOrWhiteSpace(txtTenSan.Text) ||
                cboLoaiSan.SelectedIndex < 0 ||
                cboTinhTrang.SelectedIndex < 0 ||
                string.IsNullOrWhiteSpace(txtDiaChi.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin Sân (Tên, Loại, Địa chỉ, Tình trạng).",
                    "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Bước 2: Kiểm tra danh sách bảng giá
            // Quy định: Phải có ít nhất một khung giờ giá cho sân mới
            if (_dsBangGia.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm ít nhất một khung giờ và đơn giá cho sân.",
                    "Thiếu dữ liệu bảng giá", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Bước 3: Xác nhận với người dùng trước khi lưu
            var confirm = MessageBox.Show("Bạn có chắc chắn muốn lưu thông tin sân này không?",
                "Xác nhận lưu", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                try
                {
                    // Bước 4: Đóng gói dữ liệu vào Model (SanEntity)
                    // Ở bước này, nếu bạn đã có Database, bạn sẽ gọi Service để Save.
                    // Hiện tại, chúng ta sẽ mô phỏng việc lưu thành công.

                    string maSanMoi = txtMaSan.Text; // Mã đã phát sinh ở nút "Thêm" hoặc phát sinh tại đây

                    // Giả lập lưu dữ liệu:
                    // _sanService.Save(new SanEntity { ... });

                    MessageBox.Show($"Đã lưu thành công sân {txtTenSan.Text} vào hệ thống!",
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Có lỗi xảy ra khi lưu: {ex.Message}", "Lỗi hệ thống");
                }
            }
        }
    }
}
