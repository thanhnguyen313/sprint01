# Tổng Quan Hệ Thống – Quản Lý Sân Thể Thao

## 1. Giới thiệu

**Tên dự án:** SE104 – Quản Lý Sân Thể Thao (SportFieldManagement)  
**Nền tảng:** WPF Desktop Application (.NET 8)  
**Ngôn ngữ:** C# + XAML  
**Mục tiêu:** Phần mềm quản lý sân thể thao cho cơ sở kinh doanh, hỗ trợ tiếp nhận sân, quản lý bảng giá theo loại ngày và khung giờ.

---

## 2. Công nghệ sử dụng

| Thành phần       | Công nghệ                         |
| ---------------- | --------------------------------- |
| Framework        | .NET 8 (WPF – Windows Only)       |
| UI               | XAML + WPF Controls               |
| IDE              | Visual Studio 2022 (v17+)         |
| Build System     | MSBuild / dotnet CLI               |
| Version Control  | Git                                |

---

## 3. Cấu trúc thư mục hiện tại

```
SE104_SportFieldManagement/
├── .gitignore
├── docs/                          ← (thư mục tài liệu - mới tạo)
└── QuanLySan/
    ├── QuanLySan.sln              ← Solution file
    └── QuanLySan/
        ├── QuanLySan.csproj       ← Project file (.NET 8, WPF)
        ├── App.xaml               ← Application entry point
        ├── App.xaml.cs            ← App code-behind (trống)
        ├── TiepNhanSanWindow.xaml     ← Giao diện tiếp nhận sân
        └── TiepNhanSanWindow.xaml.cs  ← Logic xử lý (code-behind)
```

---

## 4. Chức năng hiện tại

### 4.1 Tiếp nhận sân (`TiepNhanSanWindow`)
Đây là màn hình **duy nhất** hiện tại. Bao gồm:

- **Thông tin sân:** Mã sân (tự động), Tên sân, Loại sân, Địa chỉ, Ghi chú, Tình trạng.
- **Bảng giá theo loại ngày:** Cho phép quản lý đơn giá theo khung giờ và loại ngày.
  - Thêm dòng giá mới (Giờ bắt đầu, Giờ kết thúc, Loại ngày, Đơn giá)
  - Xóa dòng giá đã chọn
  - (Placeholder) Lưu bảng giá

### 4.2 Quy định nghiệp vụ
- **Loại sân:** Sân bóng đá, Sân cầu lông, Sân pickleball
- **Tình trạng sân:** Hoạt động, Bảo trì
- **Loại ngày & đơn giá mặc định:**
  - Thường: 50,000 VNĐ
  - Cuối tuần: 70,000 VNĐ
  - Lễ: 100,000 VNĐ
- **Validation:**
  - Giờ bắt đầu phải nhỏ hơn giờ kết thúc
  - Các khung giờ không được chồng lấn
  - Giờ theo định dạng HH:mm

---

## 5. Kiến trúc hiện tại: Code-Behind

Hiện tại phần mềm sử dụng kiến trúc **Code-Behind** thuần, tất cả logic nằm trong file `.xaml.cs`:

```
TiepNhanSanWindow.xaml ──→ Giao diện (View)
TiepNhanSanWindow.xaml.cs ──→ Logic + Data + Validation (tất cả)
```

**Vấn đề:**
- Logic nghiệp vụ, validation, và quản lý dữ liệu đều trộn lẫn trong code-behind
- Khó test, khó mở rộng, khó bảo trì
- Không tái sử dụng được logic

→ **Giải pháp:** Tái cấu trúc theo kiến trúc **MVVM** (xem tài liệu `03_kien_truc_mvvm.md`)
