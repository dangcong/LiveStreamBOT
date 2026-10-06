# Decisions

## D001 — Giữ branch chính là `master`

Repository mới giữ branch `master` để không làm thay đổi workflow từ repository nguồn.

## D002 — Redact secret trước khi publish

Lịch sử Git được rewrite để loại `BaseSource.API/appsettings.json` có secret. Bản config hiện tại dùng placeholder; secret thật phải được cấp ngoài Git.

## D003 — Chỉ sử dụng Telegram cho thông báo

Hệ thống chỉ gửi thông báo qua Telegram. Cấu hình, package dependency và trường nhập cho các kênh thông báo khác không thuộc runtime hiện tại.

## D006 — Cảnh báo BOT không hoạt động qua Telegram

API kiểm tra BOT hằng ngày lúc 08:00 giờ Việt Nam (`Asia/Ho_Chi_Minh`), dùng ngưỡng `InactiveBotDays` (mặc định 10 ngày). Chỉ khi số ngày không hoạt động lớn hơn ngưỡng mới gửi một cảnh báo qua Telegram Bot cho danh sách staff và manager phụ trách. ID BOT đã được cảnh báo được lưu nguyên tử trong file `inactive-bot-alert-state.json` tại thư mục local application data (hoặc đường dẫn `InactiveBotAlertStatePath`); khi BOT hoạt động trở lại hoặc stream dài hạn đang chạy thì ID được gỡ để có thể cảnh báo ở đợt sau. Cơ chế này không yêu cầu thay đổi schema hoặc chạy migration database.

## D007 — Thumbnail cấu hình stream lưu ngoài release

BOT trích một frame ở khoảng giữa video sau khi tải file, rồi gửi riêng qua SignalR; thao tác thất bại không được làm gián đoạn livestream. API chỉ nhận ảnh tối đa 512 KiB khi `BotId` và `ConnectionId` khớp với BOT được gán stream. JPEG lưu trên filesystem ngoài thư mục release, có thể đặt đường dẫn bằng `StreamThumbnailStoragePath`; production dùng `/srv/livestreambot/state/stream-thumbnails`. AppUI proxy ảnh qua endpoint cần đăng nhập và API chỉ trả ảnh cho chủ stream. Khi stream bị xóa hoặc đổi video nguồn, thumbnail tương ứng được dọn mà không cần migration database.
