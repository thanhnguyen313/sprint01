# Logic Nghiệp Vụ & Quy Định

## 1. Quy định loại sân

Hệ thống hỗ trợ 3 loại sân:

| Loại sân       | Mã enum        | Display Text      |
| -------------- | -------------- | ----------------- |
| Sân bóng đá    | `SanBongDa`    | "Sân bóng đá"    |
| Sân cầu lông   | `SanCauLong`   | "Sân cầu lông"   |
| Sân pickleball  | `SanPickleball`| "Sân pickleball"  |

---

## 2. Tình trạng sân

| Tình trạng | Mã enum     | Display Text  |
| ---------- | ----------- | ------------- |
| Hoạt động  | `HoatDong`  | "Hoạt động"  |
| Bảo trì    | `BaoTri`    | "Bảo trì"    |

---

## 3. Quy định đơn giá theo loại ngày (QĐ1)

| Loại ngày  | Đơn giá (VNĐ) | Ghi chú                |
| ---------- | -------------- | ---------------------- |
| Thường     | 50,000         | Ngày trong tuần (T2-T6)|
| Cuối tuần  | 70,000         | Thứ 7, Chủ nhật        |
| Lễ         | 100,000        | Ngày lễ, Tết           |

**Hành vi:** Khi người dùng chọn "Loại ngày" trong ComboBox, đơn giá sẽ tự động hiển thị tương ứng (ReadOnly).

---

## 4. Validation bảng giá

### 4.1 Kiểm tra đầy đủ thông tin
- Giờ bắt đầu: **Bắt buộc**, không để trống
- Giờ kết thúc: **Bắt buộc**, không để trống
- Loại ngày: **Bắt buộc**, phải chọn từ ComboBox

### 4.2 Kiểm tra định dạng giờ
- Format: `HH:mm` (VD: `07:00`, `22:00`)
- Parse bằng `TimeSpan.TryParse()`
- Nếu sai format → Hiện MessageBox cảnh báo

### 4.3 Kiểm tra logic giờ
- **Giờ bắt đầu < Giờ kết thúc** (bắt buộc)
- Nếu giờ bắt đầu >= giờ kết thúc → Hiện MessageBox cảnh báo

### 4.4 Kiểm tra chồng lấn khung giờ
- Mỗi dòng giá mới phải **không chồng lấn** với bất kỳ dòng nào đã có
- Điều kiện chồng lấn: `gioBatDau_moi < gioKetThuc_cu AND gioKetThuc_moi > gioBatDau_cu`
- Nếu chồng lấn → Hiện MessageBox chỉ rõ dòng bị chồng

---

## 5. Luồng xử lý chính

### 5.1 Thêm dòng giá
```
Người dùng nhập: GiờBĐ, GiờKT, chọn LoạiNgày
        │
        ▼
[Validate đầy đủ thông tin] ──(thiếu)──> MessageBox "Thiếu thông tin"
        │ (đủ)
        ▼
[Parse TimeSpan] ──(lỗi format)──> MessageBox "Sai định dạng"
        │ (OK)
        ▼
[GiờBĐ < GiờKT?] ──(không)──> MessageBox "Sai khung giờ"
        │ (có)
        ▼
[Kiểm tra chồng lấn] ──(chồng)──> MessageBox "Khung giờ chồng lấn"
        │ (không chồng)
        ▼
[Lấy đơn giá theo LoạiNgày]
        │
        ▼
[Thêm DonGiaItem vào ObservableCollection]
        │
        ▼
[Reset form nhập: xóa GiờBĐ, GiờKT, bỏ chọn LoạiNgày]
```

### 5.2 Xóa dòng giá
```
Người dùng chọn 1 dòng trong DataGrid
        │
        ▼
[Có dòng được chọn?] ──(không)──> MessageBox "Chưa chọn dòng"
        │ (có)
        ▼
[Xóa khỏi ObservableCollection]
        │
        ▼
[Cập nhật lại STT cho tất cả dòng còn lại]
        │
        ▼
[Refresh DataGrid]
```

### 5.3 Chọn loại ngày
```
Người dùng chọn LoạiNgày trong ComboBox
        │
        ▼
[Lookup Dictionary _bangGiaTheoLoaiNgay]
        │
        ▼
[Cập nhật txtDonGia (ReadOnly)] 
```

---

## 6. MessageBox Messages

| Tình huống              | Title               | Message                                                            | Icon      |
| ----------------------- | -------------------- | ------------------------------------------------------------------ | --------- |
| Thiếu thông tin         | "Thiếu thông tin"    | "Vui lòng nhập đầy đủ Giờ bắt đầu, Giờ kết thúc và Loại ngày."   | Warning   |
| Sai định dạng giờ       | "Sai định dạng"      | "Giờ bắt đầu / kết thúc không hợp lệ.\n...HH:mm (VD: 07:00)"     | Warning   |
| Giờ không hợp lệ        | "Sai khung giờ"      | "Giờ bắt đầu phải nhỏ hơn Giờ kết thúc."                          | Warning   |
| Khung giờ chồng lấn     | "Khung giờ chồng lấn"| "Khung giờ {X}-{Y} bị chồng lấn với dòng {STT} ({A}-{B})."       | Warning   |
| Chưa chọn dòng để xóa   | "Chưa chọn dòng"    | "Vui lòng chọn một dòng trong bảng để xóa."                       | Information|
