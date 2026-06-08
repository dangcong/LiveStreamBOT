using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BaseSource.Shared.Bot;
using BaseSouce.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BaseSource.API.Controllers
{
    /// <summary>
    /// API phục vụ tự động cập nhật bot (anonymous). Lưu/trả file zip theo từng RuntimeIdentifier
    /// để hỗ trợ nhiều nền tảng (win-x64, linux-x64...).
    /// </summary>
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class BotUpdateController : ControllerBase
    {
        const string botName = "LiveStreamBOT";
        const int keepLatestVersions = 2;

        readonly IWebHostEnvironment _webHostEnvironment;
        readonly ILogger _logger;
        public BotUpdateController(
            IWebHostEnvironment webHostEnvironment
            , ILogger<BotUpdateController> logger
            )
        {
            this._webHostEnvironment = webHostEnvironment;
            this._logger = logger;
        }

        string GetWebRootPath()
        {
            // WebRootPath có thể null nếu wwwroot chưa tồn tại lúc khởi động.
            return _webHostEnvironment.WebRootPath ?? Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");
        }

        [HttpGet("GetLatestVersion")]
        public Task<ApiResult<BotVersionVM>> GetLatestVersionAsync(
            [FromQuery] string rid,
            CancellationToken cancellationToken = default
            )
        {
            if (string.IsNullOrWhiteSpace(rid))
            {
                Response.StatusCode = 400;
                return Task.FromResult<ApiResult<BotVersionVM>>(new ApiErrorResult<BotVersionVM>("Thiếu tham số 'rid'"));
            }

            string dirPath = Path.Combine(GetWebRootPath(), "updates", botName, rid);
            if (!Directory.Exists(dirPath))
            {
                return Task.FromResult<ApiResult<BotVersionVM>>(new ApiErrorResult<BotVersionVM>($"Không tìm thấy bản cập nhật cho rid '{rid}'"));
            }

            var directoryInfo = new DirectoryInfo(dirPath);
            var files = directoryInfo.GetFiles("*.zip");
            if (files.Length == 0)
                return Task.FromResult<ApiResult<BotVersionVM>>(new ApiErrorResult<BotVersionVM>("Không tìm thấy file version nào."));

            var latestVersionFile = files
                .Select(f => new
                {
                    FileInfo = f,
                    VersionParsed = TryParseVersion(Path.GetFileNameWithoutExtension(f.Name))
                })
                .Where(x => x.VersionParsed != null)
                .OrderByDescending(x => x.VersionParsed)
                .FirstOrDefault();
            if (latestVersionFile == null)
            {
                return Task.FromResult<ApiResult<BotVersionVM>>(new ApiErrorResult<BotVersionVM>("Tên file không đúng định dạng Semantic Versioning."));
            }

            var vm = new BotVersionVM()
            {
                Version = latestVersionFile.VersionParsed.ToString(),
                FileName = latestVersionFile.FileInfo.Name,
                Rid = rid,
                DownloadUrl = $"updates/{botName}/{rid}/{latestVersionFile.FileInfo.Name}",
                LastUpdated = latestVersionFile.FileInfo.LastWriteTime
            };
            return Task.FromResult<ApiResult<BotVersionVM>>(new ApiSuccessResult<BotVersionVM>(vm));
        }

        Version TryParseVersion(string versionString)
        {
            return Version.TryParse(versionString, out Version v) ? v : null;
        }

        [HttpPost("Publish")]
        [DisableRequestSizeLimit]
        public async Task<ApiResult<object>> PublishAsync(
            [FromQuery] string version,
            [FromQuery] string rid,
            CancellationToken cancellationToken = default
            )
        {
            if (string.IsNullOrWhiteSpace(version) || TryParseVersion(version) == null)
            {
                Response.StatusCode = 400;
                return new ApiErrorResult<object>("Tham số 'version' không hợp lệ");
            }
            if (string.IsNullOrWhiteSpace(rid))
            {
                Response.StatusCode = 400;
                return new ApiErrorResult<object>("Thiếu tham số 'rid'");
            }
            if (Request.ContentLength is null || Request.ContentLength <= 0 || !Request.Headers.ContentType.Contains("application/zip"))
            {
                Response.StatusCode = 400;
                return new ApiErrorResult<object>("Body must be application/zip");
            }
            try
            {
                string dirPath = Path.Combine(GetWebRootPath(), "updates", botName, rid);
                Directory.CreateDirectory(dirPath);
                string filePath = Path.Combine(dirPath, $"{version}.zip");
                if (System.IO.File.Exists(filePath))
                {
                    Response.StatusCode = 409;
                    return new ApiErrorResult<object>("This version already exists");
                }

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    await Request.Body.CopyToAsync(fileStream, cancellationToken);
                }

                var filesToDelete = Directory.GetFiles(dirPath, "*.zip")
                    .Select(path => new
                    {
                        Path = path,
                        Version = TryParseVersion(Path.GetFileNameWithoutExtension(path))
                    })
                    .Where(x => x.Version != null)
                    .OrderByDescending(x => x.Version)
                    .Skip(keepLatestVersions);
                foreach (var file in filesToDelete)
                {
                    try
                    {
                        System.IO.File.Delete(file.Path);
                        _logger.LogInformation($"Deleted old version: {file.Version} ({rid})");
                    }
                    catch
                    {
                        // Bỏ qua lỗi nếu file đang bị khóa
                    }
                }
                return new ApiSuccessResult<object>(null);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return new ApiErrorResult<object>($"{ex.Message}\r\n{ex.StackTrace}");
            }
        }
    }
}
