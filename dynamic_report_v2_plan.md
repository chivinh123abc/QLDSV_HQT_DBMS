# Kế hoạch nâng cấp: Dynamic Reporting V2 (Bi & Analytics)

## 1. Phân tích yêu cầu
- **Custom JOINs & Views**: Hỗ trợ 2 chế độ. Chế độ cơ bản (dùng các Views định nghĩa sẵn) và Chế độ nâng cao (cho phép người dùng tự do thêm JOIN). - done

- **GROUP BY & Aggregation**: Hỗ trợ các hàm thống kê `SUM`, `COUNT`, `AVG`, `MAX`, `MIN`. Nếu một báo cáo sử dụng hàm thống kê, các cột còn lại tự động đưa vào mệnh đề `GROUP BY`.
- **Bảo mật & Hiệu năng**: Chỉ cho phép thực hiện JOIN trên các cặp bảng có quan hệ khoá chính - khoá ngoại hợp lệ (được khai báo trước thông qua Metadata Config) để ngăn chặn truy vấn Cartesian Product (làm treo server).

---

## 2. Thiết kế hệ thống (Architecture)

### 2.1. Quản lý Metadata (Join Registry)
Tạo một lớp tĩnh (Static Registry) tại tầng Application/Domain để khai báo các mối quan hệ được phép JOIN.
- `TableRelationRegistry.cs`: Chứa từ điển các quan hệ hợp lệ (ví dụ: `SINHVIEN` có thể JOIN với `LOP` qua `MALOP`).
- Lợi ích: Backend tự động build câu lệnh `ON table1.key = table2.key` an toàn, frontend không cần gửi chuỗi điều kiện JOIN (ngăn chặn injection).

### 2.2. Nâng cấp DTO (Application Layer)
Sửa đổi `DynamicQueryRequestDto` để hỗ trợ cấu trúc phức tạp hơn:
```csharp
public enum AggregateType { None, Count, Sum, Avg, Max, Min }

public class ColumnSelection
{
    public string TableName { get; set; } = string.Empty; // Vì có join nên cần biết cột của bảng nào
    public string ColumnName { get; set; } = string.Empty;
    public AggregateType Aggregate { get; set; } = AggregateType.None;
    public bool IsGroupBy { get; set; }
}

public class JoinDefinition
{
    public string JoinTable { get; set; } = string.Empty;
    public string JoinType { get; set; } = "INNER"; // INNER, LEFT
}
```

### 2.3. Repository Layer (Infrastructure)
Cập nhật `DynamicReportRepository.cs` để xử lý chuỗi T-SQL phức tạp:
- **Build `SELECT`**: Nếu cột có `Aggregate != None`, chuyển thành `SUM(T.Col) AS Col_Sum`.
- **Build `FROM` & `JOIN`**: Thêm mệnh đề `JOIN` dựa trên `JoinDefinition` và thông tin cấu hình từ `TableRelationRegistry`.
- **Build `WHERE`**: Cần alias tên bảng cho các cột lọc (`T1.Col = @val`).
- **Build `GROUP BY`**: Tự động gom nhóm các cột có `IsGroupBy = true` nếu truy vấn có sử dụng hàm thống kê.

### 2.4. Web API & Frontend (UI/UX)
- **API Mới**: `/dynamic-report/relations?tableName=XYZ` (trả về danh sách các bảng được phép kết nối với bảng hiện tại).
- **Giao diện (Index.cshtml)**:
  - Thêm Toggle Button: Chế độ Cơ bản (Chỉ 1 Bảng/View) / Chế độ Nâng cao (Tự do JOIN).
  - Chế độ nâng cao: Cho phép thêm "Bảng kết nối" (chọn từ danh sách relations).
  - Lưới chọn cột: Cập nhật UI để người dùng không chỉ tick chọn cột, mà còn có dropdown kế bên để chọn "Không gom nhóm", "Group By", "Sum", "Count", v.v.

---

## 3. Các bước triển khai (Task Breakdown)

- [ ] **Phase 1: Metadata & DTOs**
  - Cập nhật `DynamicQueryRequestDto.cs` (Thêm `ColumnSelection`, `JoinDefinition`, `AggregateType`).
  - Cập nhật `FilterCondition` (thêm `TableName` để tránh trùng lặp tên cột khi JOIN).
  - Tạo `TableRelationRegistry.cs` định nghĩa sẵn các quan hệ cốt lõi (SV - Lớp, Lớp - Khoa, Điểm - SV, Điểm - Môn học, v.v.).

- [ ] **Phase 2: Data Access / Query Builder**
  - Cập nhật `DynamicReportRepository.cs`. Viết lại logic build `SELECT`, `FROM`, `JOIN`, `WHERE`, và `GROUP BY`.
  - Cập nhật hàm phân trang `OFFSET / FETCH` để hoạt động đúng với query có `GROUP BY`.

- [ ] **Phase 3: Controller APIs**
  - Cập nhật `DynamicReportController.cs` để hỗ trợ payload mới.
  - Thêm API lấy metadata quan hệ (`GetRelations`).
  - Thêm API lấy cột có hỗ trợ bảng đang join.

- [ ] **Phase 4: Frontend Giao diện & Xử lý**
  - Cập nhật `Index.cshtml` (Giao diện Advanced Mode, config aggregate).
  - Cập nhật `dynamic-report.js` (Gọi API lấy list bảng JOIN, tạo JSON payload phức tạp, render bảng kết quả tự động hiển thị alias tên cột thống kê).
  - Cập nhật hàm xuất PDF `DynamicStandardReport.cs` để tương thích với tên cột mới.

---
Vui lòng kiểm tra kế hoạch này. Bạn có đồng ý với hướng xử lý và thiết kế trên không? Nếu ok, tôi sẽ bắt đầu tiến hành Code Phase 1 & 2.
