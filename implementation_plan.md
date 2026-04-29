# Dynamic Query Builder & Reporting Implementation Plan

Triển khai tính năng xây dựng truy vấn và báo cáo động (Dynamic Query Builder & Reporting) cho hệ thống, đảm bảo an toàn chống SQL Injection, linh hoạt trong thiết kế báo cáo và tối ưu hóa hiệu suất.

## Quyết định Thiết kế (Đã chốt)

1. **Dynamic SQL ở tầng Infrastructure:** Việc build chuỗi T-SQL sẽ được thực hiện bằng C# tại `DynamicReportRepository.cs` thay vì Stored Procedure. Điều kiện lọc sẽ được map chặt chẽ qua Enum (FilterOperator) và các giá trị đầu vào sẽ được truyền an toàn qua tham số (`SqlParameter` động).
2. **Data Source (Tables/Views):** Danh sách các Table có thể truy vấn sẽ **không bị hardcode**. Hệ thống sẽ tự động gọi query (ví dụ `INFORMATION_SCHEMA.TABLES`) sử dụng Connection String của user đang đăng nhập. User có quyền truy cập bảng nào thì hệ thống mới hiển thị bảng đó.
3. **Phân trang Preview (Pagination):** API trả về dữ liệu Preview cho giao diện sẽ tích hợp OFFSET/FETCH để giới hạn số lượng record trả về, chống tràn bộ nhớ (OOM).
4. **DevExtreme/UI Component:** Project hiện tại sử dụng **Vanilla JS + Bootstrap 5**. Do đó, giao diện Query Builder ở client-side sẽ được code thủ công bằng JS thuần kết hợp giao diện Bootstrap (hoặc sử dụng một thư viện Vanilla JS nhẹ nhàng tương thích) thay vì kéo theo jQuery hay thư viện nặng khác.

---

## Các hạng mục triển khai

### 1. Application Layer (DTOs & Interfaces)
- **`DynamicQueryRequestDto.cs`**: DTO chứa `TableName`, `SelectColumns`, `Filters` (chứa `FilterOperator` Enum), `Sorting`, `Grouping`, `PageNumber`, `PageSize`.
- **`FilterOperator.cs` (Enum)**: Định nghĩa: `Equals`, `Contains`, `StartsWith`, `EndsWith`, `GreaterThan`, `LessThan`, v.v.
- **`IDynamicReportRepository.cs`**: 
  - `GetAllowedTablesAsync()`
  - `GetTableColumnsAsync(string tableName)`
  - `GetPreviewDataAsync(DynamicQueryRequestDto request)`
  - `GetReportDataAsync(DynamicQueryRequestDto request)`

### 2. Infrastructure Layer (Repositories)
- **`DynamicReportRepository.cs`**:
  - `GetAllowedTablesAsync()`: Chạy `SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES`.
  - Hàm Helper Build SQL: Nhận DTO, map `FilterOperator` ra cú pháp T-SQL, sinh danh sách `SqlParameter` động chống Injection. Thêm OFFSET/FETCH cho Preview.

### 3. Web Layer (Controllers & Reports)
- **`DynamicReportController.cs`**: Cung cấp API GET Table, GET Column, POST Preview (trả JSON) và POST GenerateReport.
- **`DynamicStandardReport.cs` & `DynamicGroupedReport.cs`**: Kế thừa `XtraReport`. Constructor nhận `DataTable` từ DB, tự động build các `XRTable`, `XRTableCell` trên `PageHeaderBand` và `DetailBand`. Tự động tính toán bề ngang để set trang dọc (Portrait) hoặc ngang (Landscape).

### 4. Giao diện (Web/Views/DynamicReport)
- **`Index.cshtml` & `dynamic-report.js`**: 
  - Layout chia 2 phần: Bên trái là Form cấu hình Query (chọn Bảng, chọn Cột, Thêm các dòng Điều kiện lọc), Bên phải là Data Preview (hiển thị Table Bootstrap).
  - Có nút "Preview Data" gọi Ajax để xem trước.
  - Có nút "Xuất PDF" (hoặc Export) để gọi API GenerateReport.

---

## Verification Plan
1. **Security:** Tuyệt đối không có SQL Injection (test bằng cách chèn nháy đơn vào filter value).
2. **Access Control:** User có role khác nhau (PGV, KHOA, SV) sẽ chỉ nhìn thấy các bảng DB tương ứng với quyền của họ.
3. **Performance:** Preview data trên bảng có hàng chục nghìn record không gây đơ hệ thống (nhờ Pagination).
4. **Report Layout:** Báo cáo xuất ra phải tự co giãn và hiển thị đầy đủ các cột được chọn, không bị mất chữ hay tràn lề.
