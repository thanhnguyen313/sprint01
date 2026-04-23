# Hướng Dẫn Tạo Lại Với MVVM – Từng Bước

## Bước 0: Chuẩn bị

```powershell
# Yêu cầu: .NET 8 SDK
dotnet --version  # phải >= 8.0

# Tạo project WPF mới
mkdir QuanLySan
cd QuanLySan
dotnet new sln -n QuanLySan
dotnet new wpf -n QuanLySan
dotnet sln add QuanLySan/QuanLySan.csproj
```

---

## Bước 1: Tạo cấu trúc thư mục

```powershell
cd QuanLySan/QuanLySan
mkdir Models
mkdir ViewModels
mkdir ViewModels/Base
mkdir Views
mkdir Resources
```

```
QuanLySan/QuanLySan/
├── Models/
├── ViewModels/
│   └── Base/
├── Views/
└── Resources/
```

---

## Bước 2: Tạo Models

### 2.1 `Models/Enums.cs`
Định nghĩa enum cho LoaiSan, TinhTrang, LoaiNgay.  
→ Xem code mẫu trong `03_kien_truc_mvvm.md` mục 3.1

### 2.2 `Models/DonGiaItem.cs`
Class chứa thông tin 1 dòng bảng giá: STT, GioBatDau, GioKetThuc, LoaiNgay, DonGia.  
→ Xem code mẫu trong `03_kien_truc_mvvm.md` mục 3.1

### 2.3 `Models/San.cs`
Class chứa thông tin sân: MaSan, TenSan, LoaiSan, DiaChi, GhiChu, TinhTrang.  
→ Xem code mẫu trong `03_kien_truc_mvvm.md` mục 3.1

---

## Bước 3: Tạo Base ViewModel Classes

### 3.1 `ViewModels/Base/ViewModelBase.cs`
Implement `INotifyPropertyChanged` với helper method `SetProperty<T>`.  
→ Xem code mẫu trong `03_kien_truc_mvvm.md` mục 3.2

### 3.2 `ViewModels/Base/RelayCommand.cs`
Implement `ICommand` với `Action<object?>` execute và `Func<object?, bool>` canExecute.  
→ Xem code mẫu trong `03_kien_truc_mvvm.md` mục 3.2

---

## Bước 4: Tạo ViewModel cho màn Tiếp Nhận Sân

### 4.1 `ViewModels/TiepNhanSanViewModel.cs`

**Properties cần tạo:**
- `MaSan` (string, ReadOnly hiển thị)
- `TenSan` (string)
- `SelectedLoaiSan` (string?)
- `DiaChi` (string)
- `GhiChu` (string)
- `SelectedTinhTrang` (string?)
- `GioBatDau` (string, mặc định "07:00")
- `GioKetThuc` (string, mặc định "22:00")
- `SelectedLoaiNgay` (string?, setter tự cập nhật DonGiaDisplay)
- `DonGiaDisplay` (string, ReadOnly)
- `SelectedDongGia` (DonGiaItem?, cho DataGrid SelectedItem)

**Collections:**
- `DanhSachLoaiSan` (List<string>)
- `DanhSachTinhTrang` (List<string>)
- `DanhSachLoaiNgay` (List<string>)
- `DsBangGia` (ObservableCollection<DonGiaItem>)

**Commands:**
- `ThemGiaCommand` → Logic thêm dòng giá (validate + add)
- `XoaDongGiaCommand` → Logic xóa dòng + cập nhật STT
- `LuuGiaCommand` → Placeholder lưu
- `CloseCommand` → Đóng cửa sổ (nhận Window parameter)

→ Xem code đầy đủ trong `03_kien_truc_mvvm.md` mục 3.2

---

## Bước 5: Tách Styles ra ResourceDictionary

### 5.1 `Resources/Styles.xaml`
Chuyển toàn bộ nội dung `<Window.Resources>` từ file XAML hiện tại vào ResourceDictionary riêng.

Bao gồm:
- Color Palette (PrimaryBrush, TextPrimary, InputBorder)
- SimpleTextBox style
- SimpleComboBox style
- FieldLabel style
- SimpleButton style
- StyledDataGrid style
- DataGridHeaderStyle style

→ Xem chi tiết styles trong `02_chi_tiet_man_hinh.md` mục 3

### 5.2 Cập nhật `App.xaml`
```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="Resources/Styles.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

---

## Bước 6: Tạo View với Data Binding

### 6.1 Di chuyển XAML
Chuyển `TiepNhanSanWindow.xaml` → `Views/TiepNhanSanWindow.xaml`

### 6.2 Cập nhật XAML cho MVVM

**Namespace:**
```xml
xmlns:vm="clr-namespace:QuanLySan.ViewModels"
```

**DataContext:**
```xml
<Window.DataContext>
    <vm:TiepNhanSanViewModel />
</Window.DataContext>
```

**Xóa `<Window.Resources>`** (đã chuyển sang Styles.xaml)

**Thay thế binding (checklist):**

- [ ] `txtMaSan` → `Text="{Binding MaSan}"`
- [ ] `txtTenSan` → `Text="{Binding TenSan, UpdateSourceTrigger=PropertyChanged}"`
- [ ] `cboLoaiSan` → `ItemsSource="{Binding DanhSachLoaiSan}" SelectedItem="{Binding SelectedLoaiSan}"`
- [ ] `txtDiaChi` → `Text="{Binding DiaChi, UpdateSourceTrigger=PropertyChanged}"`
- [ ] `txtGhiChu` → `Text="{Binding GhiChu, UpdateSourceTrigger=PropertyChanged}"`
- [ ] `cboTinhTrang` → `ItemsSource="{Binding DanhSachTinhTrang}" SelectedItem="{Binding SelectedTinhTrang}"`
- [ ] `txtGioBatDau` → `Text="{Binding GioBatDau, UpdateSourceTrigger=PropertyChanged}"`
- [ ] `txtGioKetThuc` → `Text="{Binding GioKetThuc, UpdateSourceTrigger=PropertyChanged}"`
- [ ] `cboLoaiNgay` → `ItemsSource="{Binding DanhSachLoaiNgay}" SelectedItem="{Binding SelectedLoaiNgay}"` (xóa SelectionChanged event)
- [ ] `txtDonGia` → `Text="{Binding DonGiaDisplay}"`
- [ ] `btnThemGia` → `Command="{Binding ThemGiaCommand}"` (xóa Click event)
- [ ] `btnXoaDong` → `Command="{Binding XoaDongGiaCommand}"` (xóa Click event)
- [ ] `btnLuuGia` → `Command="{Binding LuuGiaCommand}"` (xóa Click event)
- [ ] Button Close → `Command="{Binding CloseCommand}" CommandParameter="{Binding RelativeSource={RelativeSource AncestorType=Window}}"`
- [ ] `dgBangGia` → `ItemsSource="{Binding DsBangGia}" SelectedItem="{Binding SelectedDongGia}"`

### 6.3 Cập nhật Code-Behind
```csharp
public partial class TiepNhanSanWindow : Window
{
    public TiepNhanSanWindow()
    {
        InitializeComponent();
    }
}
```

### 6.4 Cập nhật `App.xaml`
```xml
StartupUri="Views/TiepNhanSanWindow.xaml"
```

---

## Bước 7: Cập nhật `.csproj` (nếu cần)

Đảm bảo cấu hình project đúng:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <RootNamespace>QuanLySan</RootNamespace>
    <AssemblyName>QuanLySan</AssemblyName>
  </PropertyGroup>
</Project>
```

---

## Bước 8: Build & Test

```powershell
# Build
dotnet build

# Run
dotnet run
```

**Checklist test:**
- [ ] Cửa sổ mở đúng, layout giống bản cũ
- [ ] ComboBox hiển thị danh sách đúng
- [ ] Chọn Loại ngày → Đơn giá tự cập nhật
- [ ] Thêm dòng giá thành công
- [ ] Validation hiện đúng message khi thiếu thông tin
- [ ] Validation giờ: format sai, giờ BD >= KT, chồng lấn
- [ ] Xóa dòng → STT cập nhật lại
- [ ] Nút đóng hoạt động

---

## Bước 9: Mở rộng (Tương lai)

Sau khi MVVM đã ổn định, có thể mở rộng:

1. **Database:** Thêm tầng `Services/` và `Data/` để kết nối SQL Server / SQLite
2. **Navigation:** Thêm `MainWindow` với menu điều hướng nhiều màn hình
3. **Dependency Injection:** Sử dụng `Microsoft.Extensions.DependencyInjection`
4. **Unit Testing:** Tạo project test riêng cho ViewModels
5. **Thêm màn hình:** Đặt hàng sân, Thanh toán, Báo cáo, Quản lý người dùng
