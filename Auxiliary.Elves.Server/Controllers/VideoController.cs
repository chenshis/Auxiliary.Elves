using Auxiliary.Elves.Api.IApiService;
using Microsoft.AspNetCore.Mvc;

namespace Auxiliary.Elves.Server.Controllers
{
    public class VideoController : AuxiliaryControllerBase
    {
        public ILoginApiService LoginApiService { get; }

        public VideoController(ILoginApiService loginApiService)
        {
            LoginApiService = loginApiService;
        }

        /// <summary>
        /// 上传视频
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<bool> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            var allowedExtensions = new[] { ".mp4", ".avi", ".mov", ".mkv", ".wmv", ".flv" , ".webm", ".ogg" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(ext))
                return false;

            var rootPath = Directory.GetCurrentDirectory();
            var uploadPath = Path.Combine("Video", "Uploads");

            // 确保目录存在
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, Guid.NewGuid().ToString());
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return true;
        }

        /// <summary>
        /// 下载视频
        /// </summary>
        /// <returns></returns>
        [HttpGet("download")]
        public async Task<IActionResult> Download()
        {
            var uploadPath = Path.Combine("Video", "Uploads");

            if (!Directory.Exists(uploadPath))
                return NotFound("没有可下载的视频");

            var files = Directory.GetFiles(uploadPath);
            if (files.Length == 0)
                return NotFound("没有可下载的视频");

            // 随机选择一个文件
            var random = new Random();
            var filePath = files[random.Next(files.Length)];

            var fileName = Path.GetFileName(filePath);
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            // 删除文件
            System.IO.File.Delete(filePath);

            // 返回文件
            return File(fileBytes, "application/octet-stream", fileName);
        }
    }
}
