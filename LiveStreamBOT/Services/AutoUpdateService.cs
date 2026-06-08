using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiveStreamBOT.Queues;
using TqkLibrary.Concurrency.TaskQueues;

namespace LiveStreamBOT.Services
{
    /// <summary>
    /// Tự động kiểm tra & áp dụng cập nhật cho bot console (cross-platform Win/Linux).
    /// Chính sách: CHỈ giải nén + restart khi bot đang rảnh (không có stream nào chạy/chờ),
    /// để không cắt ngang livestream (stream không tự chạy lại sau restart).
    /// </summary>
    internal sealed class AutoUpdateService
    {
        // Chu kỳ kiểm tra cập nhật.
        static readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(60);

        readonly BotUpdateApiClient _apiClient;
        readonly WorkQueue<LiveStreamWork> _taskQueue;
        readonly CancellationTokenSource _cts = new();

        Task? _loopTask;

        public AutoUpdateService(
            BotUpdateApiClient apiClient,
            WorkQueue<LiveStreamWork> taskQueue
            )
        {
            this._apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            this._taskQueue = taskQueue ?? throw new ArgumentNullException(nameof(taskQueue));
        }

        public void Start()
        {
            if (_loopTask is not null) return;
            _loopTask = Task.Run(() => LoopAsync(_cts.Token));
        }

        public void Stop() => _cts.Cancel();

        async Task LoopAsync(CancellationToken cancellationToken)
        {
            LogHelper.WriteLog("AutoUpdate: start");
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_checkInterval, cancellationToken);
                    await CheckUpdateAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog($"AutoUpdate: {ex.GetType().FullName}: {ex.Message}, {ex.StackTrace}");
                }
            }
            LogHelper.WriteLog("AutoUpdate: stop");
        }

        void ClearOldFile()
        {
            string rootDir = AppContext.BaseDirectory;
            foreach (var oldFile in Directory.GetFiles(rootDir, "*.old", SearchOption.AllDirectories))
            {
                try { File.Delete(oldFile); }
                catch (Exception ex) { LogHelper.WriteLog($"AutoUpdate: xóa {oldFile} lỗi: {ex.Message}"); }
            }
        }

        async Task CheckUpdateAsync(CancellationToken cancellationToken)
        {
            ClearOldFile();

            var serverVersion = await _apiClient.GetLatestVersionAsync(Singleton.Rid, cancellationToken);
            if (serverVersion is null || !Version.TryParse(serverVersion.Version, out Version? serverVer))
                return;

            if (!Version.TryParse(GitVersionInformation.AssemblySemFileVer, out Version? currentVersion))
                return;
            if (serverVer <= currentVersion)
                return;

            LogHelper.WriteLog($"AutoUpdate: có bản mới {serverVer} (hiện tại {currentVersion})");
#if DEBUG
            return; // không tự cập nhật khi dev
#else
            // CHỈ cập nhật khi bot rảnh: không có stream đang chạy hoặc đang chờ trong hàng đợi.
            // Tránh cắt ngang livestream (stream không tự chạy lại trên bot này sau restart).
            if (_taskQueue.RunningCount != 0 || _taskQueue.QueueCount != 0)
            {
                LogHelper.WriteLog($"AutoUpdate: đang bận (running={_taskQueue.RunningCount}, queue={_taskQueue.QueueCount}), hoãn cập nhật");
                return;
            }

            using (Stream memoryStream = await _apiClient.DownloadFileAsync(serverVersion.DownloadUrl, cancellationToken))
            {
                string rootDir = AppContext.BaseDirectory;
                using ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    // Bỏ qua nếu là thư mục
                    if (string.IsNullOrEmpty(entry.Name)) continue;

                    string destPath = Path.GetFullPath(Path.Combine(rootDir, entry.FullName));
                    string destDir = Path.GetDirectoryName(destPath)!;
                    if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                    if (File.Exists(destPath))
                    {
                        string oldPath = destPath + ".old";
                        if (File.Exists(oldPath)) File.Delete(oldPath); // xóa bản .old cũ nếu có
                        File.Move(destPath, oldPath);
                        LogHelper.WriteLog($"AutoUpdate: backup {entry.FullName}");
                    }
                    entry.ExtractToFile(destPath, overwrite: true);
                    LogHelper.WriteLog($"AutoUpdate: extracted {entry.FullName}");
                }
            }

            // Linux/macOS: zip tạo trên Windows không giữ Unix permission bit nên native binary vừa giải
            // nén mất +x. Cấp lại TRƯỚC restart nếu có set_permission.sh đi kèm.
            await EnsureExecutablePermissionAsync(AppContext.BaseDirectory, cancellationToken);

            RestartApplication();
#endif
        }

        /// <summary>
        /// Cấp quyền execute cho native binary sau khi giải nén bản mới (chỉ Linux/macOS). Tái dùng
        /// set_permission.sh đi kèm bản publish nếu có. Chạy bằng /bin/sh nên script không cần +x.
        /// </summary>
        static async Task EnsureExecutablePermissionAsync(string rootDir, CancellationToken cancellationToken)
        {
            if (OperatingSystem.IsWindows()) return;

            string scriptPath = Path.Combine(rootDir, "set_permission.sh");
            if (!File.Exists(scriptPath))
            {
                LogHelper.WriteLog("AutoUpdate: không thấy set_permission.sh, bỏ qua cấp quyền execute");
                return;
            }

            using Process? process = Process.Start(new ProcessStartInfo()
            {
                FileName = "/bin/sh",
                ArgumentList = { scriptPath },
                UseShellExecute = false,
                WorkingDirectory = rootDir,
            });
            if (process is null) return;

            await process.WaitForExitAsync(cancellationToken);
            LogHelper.WriteLog($"AutoUpdate: set_permission.sh exit code {process.ExitCode}");
        }

        /// <summary>
        /// Khởi động lại bản mới (cross-platform). Truyền lại nguyên command-line (gồm URL API ở args[0])
        /// để bot reconnect đúng server. An toàn vì chỉ gọi khi bot đã rảnh (không stream nào bị cắt).
        /// </summary>
        void RestartApplication()
        {
            string? processPath = Environment.ProcessPath;
            if (string.IsNullOrEmpty(processPath))
            {
                LogHelper.WriteLog("AutoUpdate: không lấy được ProcessPath, bỏ qua restart");
                return;
            }

            // GetCommandLineArgs()[0] = đường dẫn dll managed; phần sau = user args (gồm URL API).
            string[] cmdArgs = Environment.GetCommandLineArgs();
            bool isDotnetHost = string.Equals(
                Path.GetFileNameWithoutExtension(processPath), "dotnet", StringComparison.OrdinalIgnoreCase);

            var startInfo = new ProcessStartInfo()
            {
                FileName = processPath,
                UseShellExecute = false,
                WorkingDirectory = Directory.GetCurrentDirectory(),
            };
            // dotnet host: "dotnet App.dll <args...>" (khớp Run.sh) -> truyền cả dll + args.
            // apphost: "App.exe <args...>" (khớp Run.bat) -> bỏ phần dll, chỉ truyền user args.
            foreach (string arg in isDotnetHost ? cmdArgs : cmdArgs.Skip(1))
                startInfo.ArgumentList.Add(arg);

            LogHelper.WriteLog($"AutoUpdate: restart -> {processPath} {string.Join(' ', startInfo.ArgumentList)}");
            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog($"AutoUpdate: start tiến trình mới lỗi: {ex.Message}");
                return;
            }
            Environment.Exit(0);
        }
    }
}
