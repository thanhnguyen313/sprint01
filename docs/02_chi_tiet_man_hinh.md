# Chi Tiết Màn Hình & Giao Diện

## Màn hình: TiepNhanSanWindow – Tiếp Nhận Sân

**File:** `QuanLySan/QuanLySan/TiepNhanSanWindow.xaml`  
**Code-behind:** `QuanLySan/QuanLySan/TiepNhanSanWindow.xaml.cs`

---

## 1. Thông số cửa sổ

| Thuộc tính          | Giá trị                  |
| ------------------- | ------------------------ |
| Title               | Quản lí sân thể thao     |
| Width × Height      | 750 × 720                |
| ResizeMode          | NoResize                 |
| WindowStartupLocation | CenterScreen           |
| Background          | White                    |
| Font                | Segoe UI, 13px           |

---

## 2. Bảng màu (Color Palette)

| Key            | Color     | Sử dụng                     |
| -------------- | --------- | ----------------------------- |
| PrimaryBrush   | `#1B2A4A` | Header bar, GroupBox border, DataGrid header |
| TextPrimary    | `#222222` | Label text                    |
| InputBorder    | `#CCCCCC` | Border cho TextBox, ComboBox, Button, DataGrid |
| AlternatingRow | `#F9F9F9` | Dòng xen kẽ trong DataGrid   |
| ReadOnlyBg     | `#F0F0F0` | TextBox chỉ đọc, Button background |

---

## 3. Styles đã định nghĩa

### 3.1 SimpleTextBox
- Height: 28, Padding: 6,4, FontSize: 13
- Background: White, Border: `#CCCCCC`, 1px
- VerticalContentAlignment: Center

### 3.2 SimpleComboBox
- Tương tự SimpleTextBox

### 3.3 FieldLabel (TextBlock)
- FontSize: 13, FontWeight: SemiBold
- Foreground: `#222222`, Margin: 0,0,0,4

### 3.4 SimpleButton
- Height: 28, MinWidth: 80, FontSize: 13
- Padding: 12,0, Cursor: Hand
- Background: `#F0F0F0`, Border: `#CCCCCC`

### 3.5 StyledDataGrid
- AutoGenerateColumns: False, CanUserAddRows: False
- HeadersVisibility: Column, GridLinesVisibility: Horizontal
- RowHeight: 28, SelectionMode: Single
- AlternatingRowBackground: `#F9F9F9`

### 3.6 DataGridHeaderStyle
- Background: `#1B2A4A`, Foreground: White
- FontWeight: SemiBold, Height: 30
- Padding: 8,0, BorderBrush: `#2A3E6A`

---

## 4. Layout chi tiết

### 4.1 Header Bar
```
┌──────────────────────────────────────────────────┐
│  Quản lí sân thể thao                        ✕  │
│  (FontSize 18, Bold, White)           (BtnClose) │
│  Background: #1B2A4A                             │
└──────────────────────────────────────────────────┘
```

### 4.2 Row 1 – Thông tin cơ bản
```
┌──────────┬──────────┬──────────┬──────────────┐
│ Mã sân   │ Tên sân  │ Loại sân │ Địa chỉ      │
│ (0.7*)   │ (1*)     │ (1*)     │ (1.3*)        │
│ TextBox  │ TextBox  │ ComboBox │ TextBox       │
│ ReadOnly │          │          │               │
└──────────┴──────────┴──────────┴──────────────┘
Spacing: 14px giữa các cột
```

### 4.3 Row 2 – Ghi chú & Tình trạng
```
┌──────────────────────────────┬──────────────┐
│ Ghi chú (2*)                 │ Tình trạng   │
│ TextBox                      │ ComboBox(1*) │
└──────────────────────────────┴──────────────┘
Spacing: 14px
```

### 4.4 GroupBox – Bảng giá theo loại ngày
```
┌─ Bảng giá theo loại ngày ──────────────────────┐
│                                                  │
│  ┌───────────┬───────────┬──────────┬──────────┐│
│  │Giờ bắt đầu│Giờ kết    │Loại ngày │Đơn giá   ││
│  │TextBox    │thúc TB    │ComboBox  │TextBox RO ││
│  └───────────┴───────────┴──────────┴──────────┘│
│                                                  │
│                   [Thêm] [Xóa dòng] [Lưu]  ←── │
│                                                  │
│  ┌────┬──────────┬───────────┬─────────┬───────┐│
│  │STT │Giờ bắt đầu│Giờ kết thúc│Loại ngày│Đơn giá││
│  │ 1  │ 07:00    │ 09:00     │ Thường  │50,000 ││
│  │ 2  │ 17:00    │ 21:00     │ Cuối tuần│70,000││
│  └────┴──────────┴───────────┴─────────┴───────┘│
│  DataGrid (Height: 160)                          │
└──────────────────────────────────────────────────┘
```

---

## 5. Danh sách Controls & Tên

| Control         | x:Name          | Kiểu          | Mô tả                       |
| --------------- | --------------- | ------------- | ---------------------------- |
| TextBox         | txtMaSan        | SimpleTextBox | Mã sân (ReadOnly)            |
| TextBox         | txtTenSan       | SimpleTextBox | Tên sân                      |
| ComboBox        | cboLoaiSan      | SimpleComboBox| Loại sân                     |
| TextBox         | txtDiaChi       | SimpleTextBox | Địa chỉ                      |
| TextBox         | txtGhiChu       | SimpleTextBox | Ghi chú                      |
| ComboBox        | cboTinhTrang    | SimpleComboBox| Tình trạng                   |
| TextBox         | txtGioBatDau    | SimpleTextBox | Giờ bắt đầu (mặc định 07:00)|
| TextBox         | txtGioKetThuc   | SimpleTextBox | Giờ kết thúc (mặc định 22:00)|
| ComboBox        | cboLoaiNgay     | SimpleComboBox| Loại ngày (SelectionChanged) |
| TextBox         | txtDonGia       | SimpleTextBox | Đơn giá (ReadOnly)           |
| Button          | btnThemGia      | SimpleButton  | Thêm dòng giá               |
| Button          | btnXoaDong      | SimpleButton  | Xóa dòng đang chọn          |
| Button          | btnLuuGia       | SimpleButton  | Lưu bảng giá (placeholder)  |
| DataGrid        | dgBangGia       | StyledDataGrid| Bảng giá                     |

---

## 6. Sự kiện (Events)

| Event Handler              | Trigger                  | Hành vi                                       |
| -------------------------- | ------------------------ | --------------------------------------------- |
| CboLoaiNgay_SelectionChanged | ComboBox SelectionChanged | Tự động điền đơn giá theo loại ngày được chọn |
| BtnThemGia_Click           | Button Click             | Validate & thêm dòng vào bảng giá             |
| BtnXoaDongGia_Click        | Button Click             | Xóa dòng đang chọn, cập nhật STT              |
| BtnLuuGia_Click            | Button Click             | Placeholder – chưa có logic                   |
| BtnClose_Click             | Button Click             | Đóng cửa sổ                                   |

---

## 7. DataGrid Columns

| Header       | Binding Property | Width | Format    |
| ------------ | ---------------- | ----- | --------- |
| STT          | `STT`            | 50    |           |
| Giờ bắt đầu | `GioBatDau`      | *     |           |
| Giờ kết thúc | `GioKetThuc`     | *     |           |
| Loại ngày    | `LoaiNgay`       | *     |           |
| Đơn giá      | `DonGia`         | *     | N0 (số)   |
