# Kiến Trúc MVVM – Hướng Dẫn Tái Cấu Trúc

## 1. MVVM là gì?

**MVVM (Model – View – ViewModel)** là pattern phổ biến nhất cho WPF:

```
┌──────────┐    Data Binding    ┌───────────────┐    Truy vấn    ┌─────────┐
│   VIEW   │ ◄════════════════► │  VIEW MODEL   │ ◄════════════► │  MODEL  │
│  (XAML)  │    ICommand        │    (C#)       │   Xử lý data  │  (C#)   │
└──────────┘                    └───────────────┘                └─────────┘
```

- **Model:** Dữ liệu thuần (entities, business objects)
- **View:** Giao diện XAML, không chứa logic nghiệp vụ
- **ViewModel:** Cầu nối giữa View và Model, chứa logic UI, commands, properties

---

## 2. Cấu trúc thư mục MVVM đề xuất

```
QuanLySan/
├── QuanLySan.sln
└── QuanLySan/
    ├── QuanLySan.csproj
    ├── App.xaml
    ├── App.xaml.cs
    │
    ├── Models/                        ← Tầng dữ liệu
    │   ├── San.cs                     ← Entity: Sân thể thao
    │   ├── DonGiaItem.cs              ← Entity: Dòng bảng giá
    │   └── Enums.cs                   ← Enum: LoaiSan, TinhTrang, LoaiNgay
    │
    ├── ViewModels/                    ← Tầng logic
    │   ├── Base/
    │   │   ├── ViewModelBase.cs       ← BaseVM: INotifyPropertyChanged
    │   │   └── RelayCommand.cs        ← ICommand implementation
    │   └── TiepNhanSanViewModel.cs    ← ViewModel cho màn tiếp nhận sân
    │
    ├── Views/                         ← Tầng giao diện
    │   └── TiepNhanSanWindow.xaml     ← View (chỉ XAML, không code-behind logic)
    │       TiepNhanSanWindow.xaml.cs   ← Chỉ có InitializeComponent()
    │
    ├── Services/                      ← (Tương lai) Tầng dịch vụ
    │   ├── ISanService.cs             ← Interface
    │   └── SanService.cs              ← Implementation (DB, API, ...)
    │
    └── Resources/                     ← Tài nguyên dùng chung
        └── Styles.xaml                ← Resource Dictionary cho styles
```

---

## 3. Chi tiết từng thành phần

### 3.1 Models

#### `Models/San.cs`
```csharp
namespace QuanLySan.Models
{
    public class San
    {
        public string MaSan { get; set; } = "";
        public string TenSan { get; set; } = "";
        public LoaiSan LoaiSan { get; set; }
        public string DiaChi { get; set; } = "";
        public string GhiChu { get; set; } = "";
        public TinhTrang TinhTrang { get; set; }
    }
}
```

#### `Models/DonGiaItem.cs`
```csharp
namespace QuanLySan.Models
{
    public class DonGiaItem
    {
        public int STT { get; set; }
        public string GioBatDau { get; set; } = "";
        public string GioKetThuc { get; set; } = "";
        public string LoaiNgay { get; set; } = "";
        public decimal DonGia { get; set; }
    }
}
```

#### `Models/Enums.cs`
```csharp
namespace QuanLySan.Models
{
    public enum LoaiSan
    {
        SanBongDa,
        SanCauLong,
        SanPickleball
    }

    public enum TinhTrang
    {
        HoatDong,
        BaoTri
    }

    public enum LoaiNgay
    {
        Thuong,
        CuoiTuan,
        Le
    }
}
```

---

### 3.2 ViewModels

#### `ViewModels/Base/ViewModelBase.cs`
```csharp
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QuanLySan.ViewModels.Base
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
```

#### `ViewModels/Base/RelayCommand.cs`
```csharp
using System.Windows.Input;

namespace QuanLySan.ViewModels.Base
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object? parameter) => _execute(parameter);
    }
}
```

#### `ViewModels/TiepNhanSanViewModel.cs`
```csharp
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using QuanLySan.Models;
using QuanLySan.ViewModels.Base;

namespace QuanLySan.ViewModels
{
    public class TiepNhanSanViewModel : ViewModelBase
    {
        // ═══ QUY ĐỊNH 1: Đơn giá theo loại ngày ═══
        private static readonly Dictionary<string, decimal> BangGiaTheoLoaiNgay = new()
        {
            { "Thường",    50_000 },
            { "Cuối tuần", 70_000 },
            { "Lễ",        100_000 }
        };

        // ═══ PROPERTIES (Two-way binding) ═══
        private string _maSan = "";
        public string MaSan
        {
            get => _maSan;
            set => SetProperty(ref _maSan, value);
        }

        private string _tenSan = "";
        public string TenSan
        {
            get => _tenSan;
            set => SetProperty(ref _tenSan, value);
        }

        private string? _selectedLoaiSan;
        public string? SelectedLoaiSan
        {
            get => _selectedLoaiSan;
            set => SetProperty(ref _selectedLoaiSan, value);
        }

        private string _diaChi = "";
        public string DiaChi
        {
            get => _diaChi;
            set => SetProperty(ref _diaChi, value);
        }

        private string _ghiChu = "";
        public string GhiChu
        {
            get => _ghiChu;
            set => SetProperty(ref _ghiChu, value);
        }

        private string? _selectedTinhTrang;
        public string? SelectedTinhTrang
        {
            get => _selectedTinhTrang;
            set => SetProperty(ref _selectedTinhTrang, value);
        }

        // ─── Bảng giá fields ───
        private string _gioBatDau = "07:00";
        public string GioBatDau
        {
            get => _gioBatDau;
            set => SetProperty(ref _gioBatDau, value);
        }

        private string _gioKetThuc = "22:00";
        public string GioKetThuc
        {
            get => _gioKetThuc;
            set => SetProperty(ref _gioKetThuc, value);
        }

        private string? _selectedLoaiNgay;
        public string? SelectedLoaiNgay
        {
            get => _selectedLoaiNgay;
            set
            {
                if (SetProperty(ref _selectedLoaiNgay, value))
                {
                    // Tự động cập nhật đơn giá khi chọn loại ngày
                    if (value != null && BangGiaTheoLoaiNgay.TryGetValue(value, out decimal gia))
                        DonGiaDisplay = gia.ToString("N0");
                    else
                        DonGiaDisplay = "";
                }
            }
        }

        private string _donGiaDisplay = "";
        public string DonGiaDisplay
        {
            get => _donGiaDisplay;
            set => SetProperty(ref _donGiaDisplay, value);
        }

        private DonGiaItem? _selectedDongGia;
        public DonGiaItem? SelectedDongGia
        {
            get => _selectedDongGia;
            set => SetProperty(ref _selectedDongGia, value);
        }

        // ═══ COLLECTIONS ═══
        public List<string> DanhSachLoaiSan { get; } = new() { "Sân bóng đá", "Sân cầu lông", "Sân pickleball" };
        public List<string> DanhSachTinhTrang { get; } = new() { "Hoạt động", "Bảo trì" };
        public List<string> DanhSachLoaiNgay { get; } = new() { "Thường", "Cuối tuần", "Lễ" };
        public ObservableCollection<DonGiaItem> DsBangGia { get; } = new();

        // ═══ COMMANDS ═══
        public ICommand ThemGiaCommand { get; }
        public ICommand XoaDongGiaCommand { get; }
        public ICommand LuuGiaCommand { get; }
        public ICommand CloseCommand { get; }

        // ═══ CONSTRUCTOR ═══
        public TiepNhanSanViewModel()
        {
            ThemGiaCommand = new RelayCommand(_ => ThemGia());
            XoaDongGiaCommand = new RelayCommand(_ => XoaDongGia());
            LuuGiaCommand = new RelayCommand(_ => LuuGia());
            CloseCommand = new RelayCommand(p => CloseWindow(p));
        }

        // ═══ COMMAND IMPLEMENTATIONS ═══
        private void ThemGia()
        {
            // Validate: không để trống
            if (string.IsNullOrWhiteSpace(GioBatDau) ||
                string.IsNullOrWhiteSpace(GioKetThuc) ||
                SelectedLoaiNgay == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Giờ bắt đầu, Giờ kết thúc và Loại ngày.",
                    "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Parse giờ
            if (!TimeSpan.TryParse(GioBatDau.Trim(), out TimeSpan gioBD) ||
                !TimeSpan.TryParse(GioKetThuc.Trim(), out TimeSpan gioKT))
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

            // Validate: khung giờ không chồng lấn
            foreach (var dong in DsBangGia)
            {
                if (TimeSpan.TryParse(dong.GioBatDau, out TimeSpan bdCu) &&
                    TimeSpan.TryParse(dong.GioKetThuc, out TimeSpan ktCu))
                {
                    if (gioBD < ktCu && gioKT > bdCu)
                    {
                        MessageBox.Show(
                            $"Khung giờ {GioBatDau} - {GioKetThuc} bị chồng lấn với dòng {dong.STT} ({dong.GioBatDau} - {dong.GioKetThuc}).",
                            "Khung giờ chồng lấn", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }

            // Lấy đơn giá từ quy định
            decimal donGia = BangGiaTheoLoaiNgay[SelectedLoaiNgay];

            // Thêm dòng mới
            DsBangGia.Add(new DonGiaItem
            {
                STT = DsBangGia.Count + 1,
                GioBatDau = GioBatDau.Trim(),
                GioKetThuc = GioKetThuc.Trim(),
                LoaiNgay = SelectedLoaiNgay,
                DonGia = donGia
            });

            // Reset ô nhập
            GioBatDau = "";
            GioKetThuc = "";
            SelectedLoaiNgay = null;
            DonGiaDisplay = "";
        }

        private void XoaDongGia()
        {
            if (SelectedDongGia != null)
            {
                DsBangGia.Remove(SelectedDongGia);
                // Cập nhật lại STT
                for (int i = 0; i < DsBangGia.Count; i++)
                    DsBangGia[i].STT = i + 1;
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng trong bảng để xóa.",
                    "Chưa chọn dòng", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LuuGia()
        {
            // TODO: Implement save logic (database, file, API...)
            MessageBox.Show("Đã lưu bảng giá thành công!", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseWindow(object? parameter)
        {
            if (parameter is Window window)
                window.Close();
        }
    }
}
```

---

### 3.3 Views (XAML cập nhật cho MVVM)

#### Thay đổi chính trong `TiepNhanSanWindow.xaml`:

**1. Đặt DataContext:**
```xml
<Window ...>
    <Window.DataContext>
        <vm:TiepNhanSanViewModel />
    </Window.DataContext>
```

**2. Thay Event Handler bằng Data Binding:**

| Trước (Code-Behind)                     | Sau (MVVM)                                          |
| --------------------------------------- | --------------------------------------------------- |
| `x:Name="txtMaSan"` + code-behind      | `Text="{Binding MaSan}"`                             |
| `x:Name="txtTenSan"` + code-behind     | `Text="{Binding TenSan}"`                            |
| `cboLoaiSan.ItemsSource = new[]{...}`   | `ItemsSource="{Binding DanhSachLoaiSan}"`            |
| `SelectionChanged="CboLoaiNgay_..."`   | `SelectedItem="{Binding SelectedLoaiNgay}"`          |
| `Click="BtnThemGia_Click"`             | `Command="{Binding ThemGiaCommand}"`                 |
| `Click="BtnXoaDongGia_Click"`          | `Command="{Binding XoaDongGiaCommand}"`              |
| `Click="BtnClose_Click"`               | `Command="{Binding CloseCommand}" CommandParameter="{Binding RelativeSource={RelativeSource AncestorType=Window}}"` |
| `dgBangGia.ItemsSource = _dsBangGia`   | `ItemsSource="{Binding DsBangGia}"`                  |

**3. Code-behind chỉ còn:**
```csharp
public partial class TiepNhanSanWindow : Window
{
    public TiepNhanSanWindow()
    {
        InitializeComponent();
    }
}
```

---

### 3.4 Resources – Tách Styles ra file riêng

#### `Resources/Styles.xaml`
Chuyển toàn bộ `<Window.Resources>` ra thành `ResourceDictionary`:
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <!-- Chuyển tất cả styles từ Window.Resources vào đây -->
    <SolidColorBrush x:Key="PrimaryBrush" Color="#1B2A4A"/>
    <SolidColorBrush x:Key="TextPrimary" Color="#222222"/>
    <SolidColorBrush x:Key="InputBorder" Color="#CCCCCC"/>
    
    <!-- ... tất cả styles ... -->
    
</ResourceDictionary>
```

#### `App.xaml` – Tham chiếu ResourceDictionary:
```xml
<Application ...>
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="Resources/Styles.xaml"/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

---

## 4. Mapping Code-Behind → MVVM

Bảng liệt kê chi tiết việc chuyển đổi từng phần:

| # | Code-Behind (hiện tại)                    | MVVM (mới)                              | Nằm ở         |
|---|-------------------------------------------|-----------------------------------------|----------------|
| 1 | `DonGiaItem` class trong `.xaml.cs`       | `Models/DonGiaItem.cs`                  | Model          |
| 2 | `_dsBangGia` ObservableCollection         | `DsBangGia` property trong ViewModel   | ViewModel      |
| 3 | `_bangGiaTheoLoaiNgay` Dictionary         | `BangGiaTheoLoaiNgay` static readonly  | ViewModel      |
| 4 | ComboBox ItemsSource gán trong constructor| Properties: `DanhSachLoaiSan`, etc.    | ViewModel      |
| 5 | `CboLoaiNgay_SelectionChanged`            | `SelectedLoaiNgay` setter logic        | ViewModel      |
| 6 | `BtnThemGia_Click`                        | `ThemGiaCommand` → `ThemGia()`         | ViewModel      |
| 7 | `BtnXoaDongGia_Click`                     | `XoaDongGiaCommand` → `XoaDongGia()`  | ViewModel      |
| 8 | `BtnLuuGia_Click`                         | `LuuGiaCommand` → `LuuGia()`          | ViewModel      |
| 9 | `BtnClose_Click`                          | `CloseCommand` + CommandParameter       | ViewModel      |
| 10| Styles trong `<Window.Resources>`         | `Resources/Styles.xaml`                 | ResourceDict   |

---

## 5. Lợi ích sau khi chuyển MVVM

| Tiêu chí        | Code-Behind          | MVVM                        |
| --------------- | -------------------- | ----------------------------|
| Testability     | ❌ Khó test          | ✅ Test ViewModel riêng     |
| Reusability     | ❌ Logic gắn chặt UI | ✅ ViewModel tái sử dụng    |
| Maintainability | ❌ File lớn, khó đọc | ✅ Tách biệt rõ ràng        |
| Scalability     | ❌ Khó mở rộng      | ✅ Thêm feature dễ dàng     |
| Team Work       | ❌ Conflict nhiều   | ✅ Designer / Dev phân chia  |
