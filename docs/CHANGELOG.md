# Changelog

## 2026-10-16

- Điều chỉnh theo yêu cầu tiếp theo: bắt buộc cả link Video và Audio khi tạo/sửa, chặn chạy stream thiếu link, thay cho quy tắc cho phép Audio trống của bản trước; sửa model trả lại form Edit khi có lỗi. Kiểm tra 16 trường hợp MVC validation và build solution đạt 0 lỗi, không chạy migration.
- Cho phép Audio để trống đúng thiết kế khi tạo/sửa stream; bỏ validation bắt buộc ngầm do nullable reference types và chuẩn hóa Audio rỗng trước khi lưu, không đổi schema database.
- Sửa phản hồi tạo luồng: tải lại danh sách khi form/API báo lỗi, hiển thị validation rõ hơn cho loại live và thời gian, và chỉ báo thành công sau khi API xác nhận lưu.
- Xác minh bằng MVC validator: Audio trống được chấp nhận khi tạo/sửa, Video bắt buộc vẫn bị từ chối khi thiếu; build toàn solution bằng SDK 8.0.426 đạt 0 lỗi.

## 2026-10-06

- Đổi thứ tự bảng cấu hình thành Action, Tên luồng, Job, BotName, Group, Live; tăng thumbnail thêm 20% và căn lên đầu ô.
- Thêm cột `Job` giữa `Group` và `Live`, hiển thị frame thumbnail trích ở giữa video sau khi BOT tải xong; lỗi thiếu `ffprobe`/`ffmpeg` không làm gián đoạn livestream.
- API lưu thumbnail ra thư mục dữ liệu bền vững (cấu hình bằng `StreamThumbnailStoragePath`), chỉ chủ sở hữu xem được qua AppUI; xóa thumbnail cùng khi xóa stream hoặc đổi video nguồn, không cần migration database.
- BOT tải thumbnail qua SignalR riêng, giới hạn 512 KiB và chỉ chấp nhận JPEG từ BOT có `BotId`/`ConnectionId` đang khớp với stream.
- Thêm systemd drop-in production để lưu thumbnail ở `/srv/livestreambot/state/stream-thumbnails`, ngoài thư mục release.

- Đổi timezone VPS production sang `Asia/Ho_Chi_Minh` (UTC+7), giữ NTP đồng bộ; restart API/AppUI để nạp timezone mới, không chạy migration.
- Đổi nhãn nút nhóm BOT từ “DS User” thành “User”.
- Sửa profile phát hành Linux BOT để upload ZIP cập nhật lên API production `liveapi.lushmedia.net` thay vì domain `.co` cũ.
- Thêm worker cảnh báo Telegram BOT không hoạt động theo `InactiveBotDays`, gửi cho staff và manager; lưu trạng thái chống gửi lặp trong file bền vững để không yêu cầu đổi schema database.
- Chuyển lịch kiểm tra cảnh báo BOT sang một lần mỗi ngày lúc 08:00 giờ Việt Nam; worker chờ đến mốc tiếp theo thay vì kiểm tra ngay khi API khởi động.
- Chỉ gửi cảnh báo khi số ngày không hoạt động lớn hơn `InactiveBotDays` (mặc định 10); đúng 10 ngày chưa gửi.
- Thu gọn nhóm 4 nút Edit/Stream/BOT/Xóa trong bảng user khoảng 30% theo chiều ngang; giữ vùng chạm tối thiểu 44px trên mobile.
- Căn giữa tiêu đề và nút “Tạo mới” của header danh sách người dùng trên mobile; căn giữa nội dung bên trong nút ở mọi kích thước.
- Chuẩn hóa URL mặc định kết nối BOT trong console, Run scripts và launch profile về `https://liveapi.lushmedia.net/` để khớp domain production.

## 2026-10-05

- Bỏ `AppUser.Password`, ổn định dữ liệu seed Identity để tránh migration vô tình thay hash/stamp tài khoản mặc định, và thêm `RemoveLegacyPlaintextPassword` chỉ xóa cột `AppUsers.Password`.
- Migration mới chưa được áp dụng vào database; cần backup trước khi triển khai để xóa dữ liệu cũ an toàn.
- Gỡ các migration/artifact triển khai cũ không còn dùng và hợp nhất bước đổi tên cột Telegram vào migration chain hiện hành.
- Chốt kích thước/chiều rộng nhóm action danh sách user cho thao tác mobile; thêm version query cho CSS Admin để tránh giữ style cũ trong cache.
- Triển khai AppUI và API lên Ubuntu production với bản sao lưu từng release; không áp dụng migration database.

- Tạo repository làm việc mới từ `minhyentcu/LiveStreamBOT`.
- Giữ các branch chính và feature branch từ source.
- Loại secret khỏi Git history trước khi publish và thêm config placeholder.
- Chỉ giữ Telegram làm kênh thông báo và chuẩn hóa migration để database mới chỉ dùng schema Telegram.
- Đổi hostname triển khai từ `livetest.lushmedia.net`/`livetestapi.lushmedia.net` sang `live.lushmedia.net`/`liveapi.lushmedia.net`; cập nhật runtime config và Cloudflare Tunnel/DNS tương ứng.
- Rà soát tĩnh UI/AppUI: phát hiện touch target nhỏ, mất focus outline, bảng tràn ngang; ghi nhận cần xử lý lưu và lộ mật khẩu dạng rõ cùng việc đồng bộ package.
- Chuẩn hóa toàn bộ project về `net8.0`, đồng bộ EF Core/ASP.NET Core dependencies theo .NET 8 và thay các ASP.NET package cũ bằng framework reference khi phù hợp.
- Cải thiện focus bàn phím, vùng chạm nút (40px desktop/44px mobile), viewport không khóa zoom; vùng bảng mobile cuộn ngang có thể focus và có nhãn trợ năng.
- Gộp đăng ký `AddControllersWithViews` trong AppUI; xác minh build solution thành công với .NET SDK 8.0.425.
- Tinh gọn action người dùng thành “Edit”, “Stream”, “BOT”; gộp tên đăng nhập, đổi mật khẩu, Telegram và Manager vào form Edit; loại bỏ modal/nút Reset Pass riêng.
- Thu gọn nhóm nút thao tác user khoảng 30% trên mobile nhưng giữ vùng chạm 44px; thêm Bootstrap trigger dự phòng và đăng ký handler Edit trước khi khởi tạo DataTables.
- Các luồng tạo/đổi/reset mật khẩu không còn ghi mật khẩu rõ vào cột legacy; bỏ hiển thị trường mật khẩu khỏi trang Admin/Manager.
- AppUI/API nhận `X-Forwarded-Proto` từ Cloudflare Tunnel trước HTTPS redirection, cố định cổng đích 443; middleware redirect chạy trước Swagger/static để cả API docs cũng bắt buộc HTTPS.

## 2026-10-07

- Đồng bộ bảng stream Admin/Manager theo thứ tự STT, Manager, Người dùng, Tên Luồng, Job, BotName, Group; thêm thumbnail Job qua API admin có kiểm tra quyền.
