import { useState } from "react";
import { Bell, Check, Trash2, Filter, Search, Clock, AlertCircle, CheckCircle, Info, Award } from "lucide-react";

type NotificationType = "info" | "success" | "warning" | "grade";

interface Notification {
  id: string;
  type: NotificationType;
  title: string;
  message: string;
  time: string;
  isRead: boolean;
}

const mockNotifications: Notification[] = [
  {
    id: "1",
    type: "grade",
    title: "Điểm mới đã được công bố",
    message: "Điểm môn Cơ sở dữ liệu (LTC001) đã được cập nhật. Xem chi tiết tại trang Điểm.",
    time: "5 phút trước",
    isRead: false,
  },
  {
    id: "2",
    type: "warning",
    title: "Nhắc nhở đăng ký học phần",
    message: "Đợt đăng ký học phần học kỳ 2 năm học 2025-2026 sẽ đóng vào 23:59 ngày 30/03/2026.",
    time: "1 giờ trước",
    isRead: false,
  },
  {
    id: "3",
    type: "info",
    title: "Thông báo lịch thi",
    message: "Lịch thi giữa kỳ môn Lập trình Web đã được cập nhật. Kiểm tra lại lịch học của bạn.",
    time: "2 giờ trước",
    isRead: true,
  },
  {
    id: "4",
    type: "success",
    title: "Đăng ký thành công",
    message: "Bạn đã đăng ký thành công lớp tín chỉ LTC045 - Lập trình Web.",
    time: "1 ngày trước",
    isRead: true,
  },
  {
    id: "5",
    type: "info",
    title: "Thay đổi phòng học",
    message: "Phòng học môn Cơ sở dữ liệu thay đổi từ A101 sang B205. Thời gian: Thứ 2, 15/04/2026.",
    time: "2 ngày trước",
    isRead: true,
  },
  {
    id: "6",
    type: "warning",
    title: "Nhắc nhở học phí",
    message: "Hạn đóng học phí học kỳ 2 là 31/03/2026. Vui lòng hoàn tất để không bị khóa tài khoản.",
    time: "3 ngày trước",
    isRead: true,
  },
  {
    id: "7",
    type: "info",
    title: "Thông báo nghỉ học",
    message: "Lớp Lập trình Web nghỉ học vào Thứ 4 (29/03/2026). Sẽ học bù vào Thứ 7 cùng tuần.",
    time: "5 ngày trước",
    isRead: true,
  },
];

export function NotificationsPage() {
  const [notifications, setNotifications] = useState<Notification[]>(mockNotifications);
  const [filter, setFilter] = useState<"all" | "unread">("all");
  const [searchQuery, setSearchQuery] = useState("");

  const unreadCount = notifications.filter((n) => !n.isRead).length;

  const filteredNotifications = notifications
    .filter((n) => {
      if (filter === "unread" && n.isRead) return false;
      if (searchQuery && !n.title.toLowerCase().includes(searchQuery.toLowerCase()) && !n.message.toLowerCase().includes(searchQuery.toLowerCase())) {
        return false;
      }
      return true;
    });

  const markAsRead = (id: string) => {
    setNotifications(notifications.map((n) => (n.id === id ? { ...n, isRead: true } : n)));
  };

  const markAllAsRead = () => {
    setNotifications(notifications.map((n) => ({ ...n, isRead: true })));
  };

  const deleteNotification = (id: string) => {
    setNotifications(notifications.filter((n) => n.id !== id));
  };

  const deleteAllRead = () => {
    setNotifications(notifications.filter((n) => !n.isRead));
  };

  const getNotificationIcon = (type: NotificationType) => {
    switch (type) {
      case "success":
        return <CheckCircle className="w-5 h-5 text-accent" />;
      case "warning":
        return <AlertCircle className="w-5 h-5 text-warning" />;
      case "grade":
        return <Award className="w-5 h-5 text-primary" />;
      default:
        return <Info className="w-5 h-5 text-blue-600" />;
    }
  };

  const getNotificationBgColor = (type: NotificationType) => {
    switch (type) {
      case "success":
        return "bg-accent/10";
      case "warning":
        return "bg-warning/10";
      case "grade":
        return "bg-primary/10";
      default:
        return "bg-blue-50";
    }
  };

  return (
    <div className="max-w-5xl mx-auto space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-semibold text-foreground flex items-center gap-3">
            <Bell className="w-7 h-7 text-primary" />
            Thông báo
            {unreadCount > 0 && (
              <span className="px-3 py-1 bg-destructive text-destructive-foreground rounded-full text-sm font-medium">
                {unreadCount} mới
              </span>
            )}
          </h2>
          <p className="text-sm text-muted-foreground mt-1">
            Quản lý tất cả thông báo và cập nhật từ hệ thống
          </p>
        </div>
      </div>

      {/* Toolbar */}
      <div className="bg-card border border-border rounded-xl p-4">
        <div className="flex flex-col lg:flex-row gap-4">
          {/* Search */}
          <div className="flex-1 relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
            <input
              type="text"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              placeholder="Tìm kiếm thông báo..."
              className="w-full pl-10 pr-4 py-2.5 border border-input bg-input-background rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>

          {/* Filter Buttons */}
          <div className="flex gap-2">
            <button
              onClick={() => setFilter("all")}
              className={`px-4 py-2.5 rounded-lg text-sm font-medium transition-colors flex items-center gap-2 ${
                filter === "all"
                  ? "bg-primary text-primary-foreground"
                  : "bg-secondary text-secondary-foreground hover:bg-secondary/80"
              }`}
            >
              <Filter className="w-4 h-4" />
              Tất cả ({notifications.length})
            </button>
            <button
              onClick={() => setFilter("unread")}
              className={`px-4 py-2.5 rounded-lg text-sm font-medium transition-colors flex items-center gap-2 ${
                filter === "unread"
                  ? "bg-primary text-primary-foreground"
                  : "bg-secondary text-secondary-foreground hover:bg-secondary/80"
              }`}
            >
              <Bell className="w-4 h-4" />
              Chưa đọc ({unreadCount})
            </button>
          </div>

          {/* Action Buttons */}
          <div className="flex gap-2">
            {unreadCount > 0 && (
              <button
                onClick={markAllAsRead}
                className="px-4 py-2.5 bg-accent text-accent-foreground rounded-lg text-sm font-medium hover:bg-accent/90 transition-colors flex items-center gap-2"
              >
                <Check className="w-4 h-4" />
                Đọc tất cả
              </button>
            )}
            {notifications.some((n) => n.isRead) && (
              <button
                onClick={deleteAllRead}
                className="px-4 py-2.5 bg-destructive text-destructive-foreground rounded-lg text-sm font-medium hover:bg-destructive/90 transition-colors flex items-center gap-2"
              >
                <Trash2 className="w-4 h-4" />
                Xóa đã đọc
              </button>
            )}
          </div>
        </div>
      </div>

      {/* Notifications List */}
      <div className="space-y-3">
        {filteredNotifications.length === 0 ? (
          <div className="bg-card border border-border rounded-xl p-12 text-center">
            <Bell className="w-16 h-16 text-muted-foreground mx-auto mb-4 opacity-50" />
            <h3 className="text-lg font-semibold text-foreground mb-2">
              {searchQuery ? "Không tìm thấy thông báo" : "Không có thông báo"}
            </h3>
            <p className="text-sm text-muted-foreground">
              {searchQuery
                ? `Không có thông báo nào khớp với "${searchQuery}"`
                : filter === "unread"
                ? "Bạn đã đọc tất cả thông báo"
                : "Chưa có thông báo nào"}
            </p>
          </div>
        ) : (
          filteredNotifications.map((notification) => (
            <div
              key={notification.id}
              className={`bg-card border border-border rounded-xl p-5 transition-all hover:shadow-md ${
                !notification.isRead ? "ring-2 ring-primary/20" : ""
              }`}
            >
              <div className="flex items-start gap-4">
                {/* Icon */}
                <div className={`w-12 h-12 rounded-full ${getNotificationBgColor(notification.type)} flex items-center justify-center flex-shrink-0`}>
                  {getNotificationIcon(notification.type)}
                </div>

                {/* Content */}
                <div className="flex-1 min-w-0">
                  <div className="flex items-start justify-between gap-4 mb-2">
                    <h3 className={`font-semibold text-foreground ${!notification.isRead ? "text-primary" : ""}`}>
                      {notification.title}
                      {!notification.isRead && (
                        <span className="ml-2 inline-block w-2 h-2 rounded-full bg-primary" />
                      )}
                    </h3>
                    <div className="flex items-center gap-2 flex-shrink-0">
                      {!notification.isRead && (
                        <button
                          onClick={() => markAsRead(notification.id)}
                          className="p-2 text-accent hover:bg-accent/10 rounded-lg transition-colors"
                          title="Đánh dấu đã đọc"
                        >
                          <Check className="w-4 h-4" />
                        </button>
                      )}
                      <button
                        onClick={() => deleteNotification(notification.id)}
                        className="p-2 text-destructive hover:bg-destructive/10 rounded-lg transition-colors"
                        title="Xóa"
                      >
                        <Trash2 className="w-4 h-4" />
                      </button>
                    </div>
                  </div>
                  <p className="text-sm text-foreground mb-3">{notification.message}</p>
                  <div className="flex items-center gap-2 text-xs text-muted-foreground">
                    <Clock className="w-3.5 h-3.5" />
                    {notification.time}
                  </div>
                </div>
              </div>
            </div>
          ))
        )}
      </div>

      {/* Stats */}
      {filteredNotifications.length > 0 && (
        <div className="bg-muted/30 border border-border rounded-xl p-4 flex items-center justify-between text-sm">
          <p className="text-muted-foreground">
            Hiển thị <span className="font-semibold text-foreground">{filteredNotifications.length}</span> thông báo
            {filter === "unread" && " chưa đọc"}
          </p>
          {unreadCount > 0 && filter === "all" && (
            <p className="text-muted-foreground">
              <span className="font-semibold text-primary">{unreadCount}</span> thông báo chưa đọc
            </p>
          )}
        </div>
      )}
    </div>
  );
}
