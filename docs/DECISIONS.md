# Decisions

## D001 — Giữ branch chính là `master`

Repository mới giữ branch `master` để không làm thay đổi workflow từ repository nguồn.

## D002 — Redact secret trước khi publish

Lịch sử Git được rewrite để loại `BaseSource.API/appsettings.json` có secret. Bản config hiện tại dùng placeholder; secret thật phải được cấp ngoài Git.

## D003 — Chỉ sử dụng Telegram cho thông báo

Hệ thống chỉ gửi thông báo qua Telegram. Cấu hình, package dependency và trường nhập cho các kênh thông báo khác không thuộc runtime hiện tại.
