# Biến định nghĩa đường dẫn cho gọn
WEB_PROJECT = src/QLDSV_HTC.Web
INFRA_PROJECT = src/QLDSV_HTC.Infrastructure

.PHONY: dev build clean db-update

format:
	dotnet format
	dotnet format --severity warn

# Chạy ứng dụng Web ở chế độ Watch (Hot Reload)
dev: format
	dotnet watch --project $(WEB_PROJECT) run

# Build toàn bộ Solution
build: format
	dotnet build

# Dọn dẹp các thư mục rác bin/obj (giúp project sạch sẽ)
clean: format
	dotnet clean
	find . -type d -name "bin" -exec rm -rf {} +
	find . -type d -name "obj" -exec rm -rf {} +

# Lệnh cập nhật Database (Dành cho đồ án DBMS của bạn)
db-update:
	dotnet ef database update --project $(INFRA_PROJECT) --startup-project $(WEB_PROJECT)