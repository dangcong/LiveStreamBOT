# Decisions

## D001 — Giữ branch chính là `master`

Repository mới giữ branch `master` để không làm thay đổi workflow từ repository nguồn.

## D002 — Redact secret trước khi publish

Lịch sử Git được rewrite để loại `BaseSource.API/appsettings.json` có secret. Bản config hiện tại dùng placeholder; secret thật phải được cấp ngoài Git.
