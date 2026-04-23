USE QLSanTheThao
GO
CREATE TABLE LOAISAN
(
	MaLoaiSan char(2) Primary key, -- BD - CL - PB: Bong da - Cau Long - Pickleball
	TenLoaiSan nvarchar(100)
)

CREATE TABLE TINHTRANG
(
	MaTinhTrang char(2) primary key, -- VD: BT - HD: Bao tri - Hoat dong
	TenTinhTrang nvarchar(100)
)
CREATE TABLE LOAINGAY
(
	MaLoaiNgay char(2) primary key, -- NT - CT - NL: Ngay Thuong - Cuoi Tuan - Ngay Le
	TenLoaiNgay nvarchar(100),
	DonGiaNgay money,
)


CREATE TABLE SAN
(
	MaSan char(6) PRIMARY KEY, -- VD: BD0235
	TenSan NVARCHAR(100),
	DiaChi NVARCHAR(100),
	MaLoaiSan char(2) references LOAISAN(MaLoaiSan),
	MaTinhTrang char(2) references TINHTRANG(MaTinhTrang) ,
)

CREATE TABLE CHITIETDATSAN
(
	MaDatSan char(12) PRIMARY KEY,
	MaSan char(6) references SAN(MaSan),
	GioBatDau time,
	GioKetThuc time,
	MaLoaiNgay char(2) references LOAINGAY(MaLoaiNgay)
)

ALTER TABLE CHITIETDATSAN
ADD CONSTRAINT CHK_ThoiGian_HopLe 
CHECK (GioBatDau < GioKetThuc);

Go

CREATE TRIGGER trg_CheckTimeOverlap
ON CHITIETDATSAN
AFTER INSERT, UPDATE
AS
BEGIN
    IF EXISTS (
        SELECT 1
        FROM CHITIETDATSAN t
        INNER JOIN inserted i ON t.MaDatSan <> i.MaDatSan
        WHERE 
            i.GioBatDau < t.GioKetThuc 
            AND t.GioBatDau < i.GioKetThuc
    )
    BEGIN
        RAISERROR ('Xung đột, Đã có lịch đặt trong khung giờ này.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
