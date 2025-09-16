using Auxiliary.Elves.Api.IApiService;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Text;

namespace Auxiliary.Elves.Server.Controllers
{
    public class VideoController : AuxiliaryControllerBase
    {
        public ILoginApiService LoginApiService { get; }
        private readonly ILogger<VideoController> _logger;

        public VideoController(ILoginApiService loginApiService,ILogger<VideoController> logger)
        {
            LoginApiService = loginApiService;
            _logger = logger;
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

            var baseDic = AppDomain.CurrentDomain.BaseDirectory;

            var videoDirectory = Path.Combine(baseDic, "videos");

            // 确保目录存在
            if (!Directory.Exists(videoDirectory))
                Directory.CreateDirectory(videoDirectory);

            var filePath = Path.Combine(videoDirectory, Guid.NewGuid().ToString());
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return true;
        }

        /// <summary>
        /// 下载视频
        /// </summary>
        /// <returns></returns>
        [HttpGet("download")]
        public async Task<IActionResult> Download(string mac)
        {
            var baseDic = AppDomain.CurrentDomain.BaseDirectory;

            var videoDirectory = Path.Combine(baseDic, "videos");

            if (!Directory.Exists(videoDirectory))
                return NotFound("没有可下载的视频");

            var files = Directory.GetFiles(videoDirectory);
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


        /// <summary>
        /// 获取视频地址
        /// </summary>
        /// <returns></returns>
        [HttpGet("getVideoUrl")]
        public async Task<string> GetVideoUrlAsync()
        {
            var baseDic = AppDomain.CurrentDomain.BaseDirectory;

            var videoDirectory = Path.Combine(baseDic, "videos");

            if (!Directory.Exists(videoDirectory))
                return "";

            var files = Directory.GetFiles(videoDirectory).Select(Path.GetFileName)
                     .ToList();

            if (!files.Any())
                return "";

            var random = new Random();
            var index = random.Next(files.Count);
            var randomFile = files[index]; // 随机文件名
            return randomFile;
            
        }


    }
}
