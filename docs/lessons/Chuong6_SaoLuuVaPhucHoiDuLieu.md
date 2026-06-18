**CHƯƠNG 6. SAO LƯU VÀ PHỤC HỒI DỮ LIỆU**

Mục đích: Đối với một Quản trị viên Cơ sở dữ liệu (DBA), sao lưu là nhiệm vụ quan trọng nhất để bảo đảm an toàn dữ liệu. Khi sự cố xảy ra, tệp sao lưu là nguồn duy nhất giúp khôi phục hệ thống. Việc mất dữ liệu có thể làm tê liệt hoạt động của doanh nghiệp, do đó kỹ năng này luôn được đặt lên hàng đầu.

### I. SAO LƯU DỮ LIỆU (BACKUP)

Lệnh backup trong SQL Server tập trung vào việc tạo bản sao của các đối tượng trong cơ sở dữ liệu (CSDL) như table, view, stored procedure, function, trigger, user, role, ràng buộc và nhật ký giao tác.

**1. Thiết bị lưu trữ (Backup Device)**
Thiết bị sao lưu có thể là một tệp lưu trên ổ đĩa cục bộ hoặc trên mạng. Một thiết bị sao lưu có thể chứa nhiều bản backup của nhiều CSDL khác nhau.

- **Tạo bằng giao diện (Management Studio):** Server Object -> Backup -> New Backup Device -> Nhập tên logic và chọn đường dẫn lưu tệp vật lý.

- **Tạo bằng T-SQL (`sp_addumpdevice`):** \* Cú pháp thiết lập thiết bị ánh xạ từ tên logic sang tên vật lý: `sp_addumpdevice [@devtype =] 'device_type', [@logicalname =] 'logical_name', [@physicalname =] 'physical_name'`.

- `device_type`: Nhận giá trị `disk` (đĩa cục bộ) hoặc `pipe` (đĩa mạng).

- `logical_name`: Tên dùng để gọi trong các lệnh BACKUP/RESTORE sau này.

- Thông tin thiết bị được lưu trong bảng `master.dbo.sysdevices`.

- **Xóa thiết bị:** Dùng lệnh `sp_dropdevice`. Nếu kết hợp tham số `@delfile = 'delfile'`, hệ thống sẽ xóa luôn cả tệp vật lý tương ứng trên ổ cứng.

**2. Các loại Sao lưu (Backup Types)**

- **Full backup:** Sao lưu toàn bộ dữ liệu tại thời điểm thực hiện. Đây là loại được sử dụng thường xuyên nhất và là nền tảng cho các loại backup khác.

- **Differential backup:** Chỉ sao lưu các trang dữ liệu có sự thay đổi kể từ lần Full Backup gần nhất.

- **Transaction log backup:** Trích xuất và sao lưu các hành động/thao tác (log records) xảy ra trên CSDL chứ không lưu bản thân dữ liệu tĩnh. Quá trình này đồng thời sẽ cắt bỏ (truncate) các log đã được backup ra khỏi tệp log hiện tại. Lưu ý: Nếu thấy tệp log tăng quá lớn, rất có thể do DBA chưa từng thực hiện backup log.

**3. Các tùy chọn quan trọng trong lệnh BACKUP bằng T-SQL**

- `INIT` / `NOINIT`: `INIT` sẽ ghi đè bản backup mới lên tệp hiện tại. `NOINIT` (mặc định) sẽ ghi nối tiếp các bản backup vào cùng một tệp (tạo thành các tập backup set có thứ tự để có thể lựa chọn phục hồi sau này).

- `NO_TRUNCATE`: Sao lưu nhật ký giao dịch nhưng không xóa (truncate) các giao tác đã hoàn tất. Tính năng này vô cùng quan trọng để lưu lại dữ liệu sát tới thời điểm CSDL xảy ra sự cố.

- `NORECOVERY`: Sao lưu xong sẽ đưa CSDL vào trạng thái 'restoring' (không cho phép người dùng truy cập). Thường dùng để sao lưu phần log cuối cùng (tail-log) ngay trước khi tiến hành chuỗi lệnh phục hồi.

- Phân quyền: Chỉ các tài khoản thuộc nhóm `sysadmin`, `db_owner`, hoặc `db_backupoperator` mới có quyền thực thi lệnh này. Lịch sử sao lưu được hệ thống ghi lại trong bảng `msdb.dbo.backupset`.

**4. Lên lịch sao lưu tự động (SQL Server Agent Jobs)**
Để tránh việc sao lưu thủ công, SQL Server Agent cho phép thiết lập các Jobs để chạy tự động.

- Truy cập SQL Server Agent -> Jobs -> New Job.

- Thiết lập tên Job và Owner (ví dụ `sa`).

- Vào thẻ Steps -> New, dán mã lệnh T-SQL (ví dụ: `BACKUP DATABASE...`) vào ô Command.

- Vào thẻ Schedules, tạo lịch biểu để chạy lặp lại định kỳ (ví dụ: chạy hàng ngày lúc 1:00 AM).

---

### II. PHỤC HỒI DỮ LIỆU (RESTORE)

Quá trình phục hồi thường bao gồm việc xóa CSDL hiện hành (nếu đang bị ghi đè, cần ngắt mọi kết nối) và tạo lại CSDL mới từ tệp backup.

**1. Các tùy chọn lệnh RESTORE bằng T-SQL**

- `RECOVERY` (Mặc định): Đưa CSDL về trạng thái 'online', cho phép user truy cập bình thường sau khi phục hồi.

- `NORECOVERY`: Báo cho SQL Server biết vẫn còn các tệp backup khác (như Differential hoặc Log) cần được áp dụng tiếp theo. CSDL sẽ giữ ở trạng thái không hoạt động ('restoring'). Tất cả các lệnh Restore trong chuỗi phải dùng `NORECOVERY`, chỉ trừ lệnh cuối cùng mới dùng `RECOVERY`.

- `STOPAT = time`: Điểm dừng phục hồi. Chỉ định CSDL phục hồi tới chính xác một mốc ngày/giờ cụ thể trong quá khứ và bỏ qua các giao dịch sau đó.

- `REPLACE`: Bắt buộc ghi đè lên CSDL hiện tại nếu tên CSDL bị trùng.

- `FILE = n`: Chọn thứ tự bản backup thứ `n` bên trong tệp vật lý chứa nhiều backup set.

**2. Kỹ thuật Phục hồi CSDL về một thời điểm trong quá khứ (Point-in-Time Recovery)**
Đây là kỹ thuật cứu hộ khi một ai đó lỡ tay thao tác sai (ví dụ chạy lệnh `DELETE` mà quên mệnh đề `WHERE`).

- **Điều kiện bắt buộc:** CSDL đang ở chế độ FULL RECOVERY, đã từng được Full Backup trước đó và tệp Log chưa từng bị Shrink (thu nhỏ).

- **Trình tự cứu hộ:**

1. Đóng lập tức mọi kết nối đến CSDL để ngăn dữ liệu rác tiếp tục tràn vào.

2. Ghi nhận lại chính xác thời điểm xảy ra lệnh lỗi (t).

3. Chạy lệnh `BACKUP LOG` khẩn cấp để sao lưu những thay đổi mới nhất.

4. Chạy lệnh `RESTORE DATABASE ... WITH NORECOVERY` từ bản Full Backup gần nhất.

5. Chạy lệnh `RESTORE LOG ... WITH STOPAT = 'thời_điểm_trước_khi_lỗi'`. Lệnh này yêu cầu hệ thống thực hiện lại mọi hành động từ file log nhưng dừng lại ngay trước lúc sự cố diễn ra.

6. Nhờ vậy, câu lệnh lỗi sẽ không được hệ thống thực thi lại, dữ liệu được khôi phục nguyên vẹn. Mở lại kết nối CSDL (chuyển sang `MULTI_USER` hoặc dùng lệnh phục hồi hoàn tất) để sử dụng lại bình thường.
