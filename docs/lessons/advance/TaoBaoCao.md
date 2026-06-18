**CHƯƠNG: TẠO BÁO CÁO VỚI DEVEXPRESS XTRAREPORT**

Mục đích: Hướng dẫn cách kết nối ứng dụng (ví dụ: C#, VB.NET) vào CSDL SQL Server và sử dụng công cụ DevExpress XtraReport để thiết kế, xuất bản các mẫu báo cáo (Report) chuyên nghiệp từ dữ liệu đã truy vấn.

---

### I. TỔNG QUAN VỀ XTRAREPORT

- **XtraReport là gì?** Là một thư viện/công cụ thiết kế báo cáo mạnh mẽ nằm trong bộ công cụ DevExpress dành cho lập trình viên .NET.
- **Ưu điểm:** \* Hỗ trợ thiết kế giao diện kéo-thả (Drag & Drop) trực quan ngay bên trong Visual Studio.
- Cho phép bind (ràng buộc) dữ liệu trực tiếp từ SQL Server (thông qua DataSet, DataTable, hoặc Entity Framework, LINQ).
- Hỗ trợ xuất file ra nhiều định dạng phổ biến: PDF, Excel (XLS/XLSX), Word (DOC/DOCX), HTML, Hình ảnh...
- Tích hợp công cụ xem trước báo cáo (Document Viewer).

### II. CÁC THÀNH PHẦN (BANDS) TRONG MỘT TRANG BÁO CÁO

Một trang báo cáo XtraReport được chia thành các dải (Bands) ngang để chứa nội dung. Hiểu rõ các Band này là yếu tố quyết định để thiết kế báo cáo đúng cấu trúc:

1. **Top Margin / Bottom Margin:** Căn lề trên và dưới của trang giấy. Thường để trống hoặc chứa logo nhỏ.
2. **Report Header:** Nằm ở phần trên cùng của báo cáo và **chỉ xuất hiện 1 lần duy nhất** ở trang đầu tiên. Thường dùng để ghi Tiêu đề báo cáo (Ví dụ: "DANH SÁCH SINH VIÊN KHOA CNTT"), tên công ty, thông số chung.
3. **Page Header:** Xuất hiện ở đầu **mỗi trang** của báo cáo. Thường dùng để hiển thị dòng tiêu đề của các cột dữ liệu (Ví dụ: STT, Mã SV, Họ Tên, Ngày Sinh).
4. **Group Header / Group Footer:** Dùng khi bạn muốn gom nhóm dữ liệu.

- _Ví dụ:_ Báo cáo danh sách sinh viên toàn trường, bạn muốn nhóm theo từng "Lớp". Group Header sẽ in ra "Lớp: 10A1", sau đó là danh sách SV lớp 10A1, kết thúc bằng Group Footer in ra "Tổng số SV lớp 10A1: 40".

5. **Detail:** Nơi quan trọng nhất, dùng để hiển thị dữ liệu chi tiết. Mỗi dòng dữ liệu (Record) lấy từ CSDL SQL Server sẽ được in lặp lại một lần trong phần Detail này.
6. **Page Footer:** Nằm ở cuối **mỗi trang**. Thường dùng để đánh số trang (Ví dụ: "Trang 1/10"), ghi ngày tháng in báo cáo.
7. **Report Footer:** Nằm ở dưới cùng của báo cáo và **chỉ xuất hiện 1 lần duy nhất** ở trang cuối cùng, ngay sau dòng Detail cuối. Thường dùng để tính tổng kết toàn bộ báo cáo (Ví dụ: "Tổng doanh thu toàn công ty", "Chữ ký Giám đốc").

### III. QUY TRÌNH 4 BƯỚC TẠO BÁO CÁO VỚI XTRAREPORT

**Bước 1: Chuẩn bị dữ liệu từ CSDL (SQL Server)**

- Trước khi làm báo cáo, bạn cần viết câu lệnh `SELECT`, hoặc tạo một `Stored Procedure` / `View` trong SQL Server để lấy đúng và đủ dữ liệu cần thiết.
- _Ví dụ:_ Để làm bảng điểm, bạn cần viết SP lấy ra (Mã SV, Họ Tên, Tên Môn, Điểm) thay vì lấy toàn bộ bảng SinhVien.

**Bước 2: Kết nối và cấu hình Dataset trong Visual Studio**

- Thêm một mục `DataSet` (.xsd) vào Project.
- Kéo bảng hoặc Stored Procedure vừa tạo từ `Server Explorer` thả vào DataSet để tạo thành một bảng dữ liệu ảo (`DataTable`).

**Bước 3: Thiết kế giao diện báo cáo (Design Report)**

- Add New Item -> Chọn DevExpress Report -> Chọn Blank Report (Báo cáo trống).
- Chỉ định `DataSource` (Nguồn dữ liệu) của Report trỏ về cái DataSet ở Bước 2.
- Chỉ định `DataMember` trỏ về đúng cái bảng (DataTable) cần hiển thị.
- Từ hộp thoại `Field List`, kéo thả các trường dữ liệu (Fields) vào dải **Detail**.
- Dùng `Toolbox` kéo thả các control (như XRLabel, XRTable, XRPictureBox) để trang trí Report Header, Page Header...

**Bước 4: Gọi và hiển thị báo cáo ra màn hình (Hiển thị qua Code C# / VB.NET)**

- Đổ dữ liệu thực tế từ SQL Server vào DataTable bằng `SqlDataAdapter`.
- Khởi tạo đối tượng Report vừa thiết kế.
- Gán nguồn dữ liệu thực tế vừa lấy được vào Report.
- Dùng lệnh hiển thị lên màn hình (Ví dụ dùng `ReportPrintTool`).

**Đoạn mã C# minh họa gọi báo cáo:**

```csharp
// 1. Khởi tạo đối tượng Report bạn đã thiết kế (Ví dụ rptDanhSachSV)
rptDanhSachSV rpt = new rptDanhSachSV();

// 2. Gán dữ liệu thực tế (dataTable đã fill dữ liệu từ SQL) vào report
rpt.DataSource = dtSinhVien;

// 3. Hiển thị báo cáo lên cửa sổ Document Viewer
ReportPrintTool printTool = new ReportPrintTool(rpt);
printTool.ShowPreviewDialog();

```

### IV. CÁC TÍNH NĂNG NÂNG CAO THƯỜNG DÙNG

1. **Truyền tham số (Parameters):** Giúp báo cáo linh hoạt. Ví dụ báo cáo "Doanh thu theo tháng", người dùng chọn tháng 5 trên Form, bạn truyền tham số 'Tháng 5' vào Report để Report chỉ in dữ liệu tháng 5.
2. **Tính toán thống kê (Calculated Fields / Summary):**
   XtraReport cung cấp tính năng Summary để tự động Đếm số dòng (Count), Tính tổng (Sum), Tính trung bình (Average) ngay trên báo cáo mà không cần viết lệnh SQL tính tổng. Thường đặt các field này ở phần `Group Footer` hoặc `Report Footer`.
3. **Định dạng theo điều kiện (Conditional Formatting):**
   Ví dụ: Thiết lập luật nếu điểm của sinh viên < 5, chữ sẽ tự động bôi màu Đỏ.
