using System.Text.Json;
using System.Text.RegularExpressions;
using Xabe.FFmpeg;

namespace AllDemo.Services
{
    public class InstagramService
    {
        private readonly HttpClient _http = new();

        public async Task<List<(string Label, string Url)>> GetFormats(string pageUrl)
        {
            // NOTE: This simplistic scraper works for many public posts/reels.
            // IG changes HTML often. Adjust selectors when needed.
            var html = await _http.GetStringAsync(pageUrl);

            // Prefer JSON-LD video object
            var ldMatches = Regex.Matches(html, "<script type=\"application/ld\\+json\">(.*?)</script>", RegexOptions.Singleline);
            foreach (Match m in ldMatches)
            {
                try
                {
                    using var doc = JsonDocument.Parse(m.Groups[1].Value);
                    if (doc.RootElement.TryGetProperty("contentUrl", out var cu))
                        return new() { ("MP4", cu.GetString()!) };
                }
                catch { /* ignore */ }
            }

            // Fallback: find m3u8/MP4 in the HTML
            var urls = new List<(string, string)>();
            foreach (Match m in Regex.Matches(html, @"https?://[^""'<> ]+"))
            {
                var u = m.Value;
                if (u.Contains(".m3u8"))
                    urls.Add(("HLS (m3u8)", u));
                else if (u.EndsWith(".mp4"))
                    urls.Add(("MP4", u));
            }

            return urls.Distinct().ToList();
        }

        public async Task<string> DownloadAsync(string mediaUrl)
        {
            var outPath = Path.Combine(Path.GetTempPath(), $"ig_{DateTime.UtcNow.Ticks}.mp4");

            if (mediaUrl.Contains(".m3u8"))
            {
                // Copy HLS into MP4 container, fix AAC ADTS if present
                var conv = FFmpeg.Conversions.New()
                    .AddParameter($"-i \"{mediaUrl}\" -c copy -bsf:a aac_adtstoasc -movflags +faststart \"{outPath}\"",
                                  ParameterPosition.PreInput);
                await conv.Start();
            }
            else
            {
                // Plain MP4 → just download bytes
                using var resp = await _http.GetAsync(mediaUrl);
                resp.EnsureSuccessStatusCode();
                await using var fs = File.Create(outPath);
                await resp.Content.CopyToAsync(fs);
            }

            return outPath;
        }
    }
}
