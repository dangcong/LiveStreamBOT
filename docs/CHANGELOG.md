# Changelog

## 2026-10-05

- Tạo repository làm việc mới từ `minhyentcu/LiveStreamBOT`.
- Giữ các branch chính và feature branch từ source.
- Loại secret khỏi Git history trước khi publish và thêm config placeholder.
- Loại bỏ Discord notification integration khỏi runtime, cấu hình, package dependency và UI; giữ Telegram là kênh thông báo duy nhất.
- Giữ các migration/SQL lịch sử có trường Discord để bảo toàn khả năng nâng cấp database cũ.
- Đổi hostname triển khai từ `livetest.lushmedia.net`/`livetestapi.lushmedia.net` sang `live.lushmedia.net`/`liveapi.lushmedia.net`; cập nhật runtime config và Cloudflare Tunnel/DNS tương ứng.
- Rà soát tĩnh UI/AppUI: phát hiện touch target nhỏ, mất focus outline, bảng tràn ngang; ghi nhận cần xử lý lưu và lộ mật khẩu dạng rõ cùng việc đồng bộ package.
