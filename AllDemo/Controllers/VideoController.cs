using AllDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.RegularExpressions;
using YoutubeExplode;

namespace AllDemo.Controllers
{
    public class VideoController : Controller
    {
        public IActionResult Downloader()
        {
            return View();
        }

        private readonly HttpClient _httpClient;
        private readonly YoutubeClient _youtube;

        public VideoController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _youtube = new YoutubeClient();
        }

        // Step 1: Preview
        [HttpPost]
        public async Task<IActionResult> Preview(string url)
        {
            if (url.Contains("youtube.com") || url.Contains("youtu.be"))
            {
                var video = await _youtube.Videos.GetAsync(url);
                var manifest = await _youtube.Videos.Streams.GetManifestAsync(video.Id);

                // 1. Try muxed streams (video+audio)
                var muxedStreams = manifest.GetMuxedStreams()
                    .Select(s => new
                    {
                        Quality = s.VideoQuality.Label ?? "Unknown",
                        Format = s.Container.Name,
                        Size = s.Size != null ? $"{Math.Round(s.Size.MegaBytes, 2)} MB" : "N/A",
                        Url = s.Url
                    });

                // 2. Fallback to video-only streams
                var videoOnlyStreams = manifest.GetVideoOnlyStreams()
                    .Select(s => new
                    {
                        Quality = s.VideoQuality.Label ?? "Unknown (Video Only)",
                        Format = s.Container.Name,
                        Size = s.Size != null ? $"{Math.Round(s.Size.MegaBytes, 2)} MB" : "N/A",
                        Url = s.Url
                    });

                // 3. Include audio-only streams too
                var audioStreams = manifest.GetAudioOnlyStreams()
                    .Select(s => new
                    {
                        Quality = "Audio Only",
                        Format = s.Container.Name,
                        Size = s.Size != null ? $"{Math.Round(s.Size.MegaBytes, 2)} MB" : "N/A",
                        Url = s.Url
                    });

                // Combine results
                var streams = muxedStreams.Concat(videoOnlyStreams).Concat(audioStreams).ToList();

                return Json(streams);
            }
            else if (url.Contains("instagram.com"))
            {
                // ---- Instagram ----
                var html = await _httpClient.GetStringAsync(url);
                var match = Regex.Match(html, "<meta property=\"og:video\" content=\"(.*?)\"");
                if (!match.Success)
                    return Json(new { error = "No video found" });

                var videoUrl = match.Groups[1].Value;

                var stream = new[]
                {
                    new { Quality = "Default", Format = "mp4", Size = "N/A", Url = videoUrl }
                };

                return Json(stream);
            }

            return Json(new { error = "Unsupported platform" });
        }

        // Step 2: Download
        [HttpGet]
        public async Task<IActionResult> Download(string url)
        {
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return NotFound("Unable to fetch video");

            var stream = await response.Content.ReadAsStreamAsync();
            var fileName = $"video_{DateTime.Now:yyyyMMddHHmmss}.mp4";

            return File(stream, "video/mp4", fileName);
        }
        // Step 1: Preview (fetch direct video link)
        [HttpPost]
        public async Task<IActionResult> InstaPreview(string url)
        {
            var html = await _httpClient.GetStringAsync(url);

            string videoUrl = null;

            // Try og:video:secure_url
            var match = Regex.Match(html, "<meta property=\"og:video:secure_url\" content=\"(.*?)\"");
            if (match.Success)
            {
                videoUrl = match.Groups[1].Value;
            }

            // Try og:video
            if (string.IsNullOrEmpty(videoUrl))
            {
                match = Regex.Match(html, "<meta property=\"og:video\" content=\"(.*?)\"");
                if (match.Success)
                {
                    videoUrl = match.Groups[1].Value;
                }
            }

            // Fallback: JSON
            if (string.IsNullOrEmpty(videoUrl))
            {
                var jsonMatch = Regex.Match(html, "\"video_url\":\"(.*?)\"");
                if (jsonMatch.Success)
                {
                    videoUrl = jsonMatch.Groups[1].Value.Replace("\\u0026", "&");
                }
            }

            if (!string.IsNullOrEmpty(videoUrl))
            {
                var stream = new
                {
                    Quality = "Default",
                    Format = "mp4",
                    Url = videoUrl
                };
                return Json(new[] { stream });
            }

            return Json(new { error = "No video found" });
        }




        // Step 2: Download
        [HttpGet]
        public async Task<IActionResult> InstaDownload(string url)
        {
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return NotFound("Unable to fetch video");

            var stream = await response.Content.ReadAsStreamAsync();
            var fileName = $"instagram_{DateTime.Now:yyyyMMddHHmmss}.mp4";

            return File(stream, "video/mp4", fileName);
        }
    }
}
