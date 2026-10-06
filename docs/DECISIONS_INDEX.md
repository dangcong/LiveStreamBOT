# Decisions Index

- D001 — Giữ branch chính là `master` để tương thích với repository nguồn.
- D002 — Không đưa secret lịch sử vào repository mới; config trong source tree chỉ dùng placeholder.
- D003 — Kênh thông báo duy nhất là Telegram.
- D004 — Chuẩn hóa tất cả project trong repository về `net8.0`; UI giữ bảng rộng trong vùng cuộn ngang trên mobile thay vì ép cột khó đọc.
- D005 — Chỉ lưu mật khẩu trong `PasswordHash`; bỏ cột legacy `AppUsers.Password` và không đổi dữ liệu seed tài khoản mặc định khi migrate.
- D006 — Cảnh báo BOT không hoạt động được kiểm tra hằng ngày lúc 08:00 giờ Việt Nam và gửi qua Telegram một lần mỗi đợt khi số ngày không hoạt động lớn hơn `InactiveBotDays`; trạng thái gửi lưu ở file bền vững, không cần migration database, và reset khi BOT hoạt động lại.
