using Xabe.FFmpeg;
using YoutubeExplode;


namespace AllDemo.Services
{
    public class YoutubeService
    {
        private readonly YoutubeClient _ytClient = new();

        public async Task<IEnumerable<(string Label, string? Url, bool IsMuxed)>> GetFormats(string url)
        {
            var manifest = await _ytClient.Videos.Streams.GetManifestAsync(url);
            var results = new List<(string,string?,bool)>();
            // Muxed
            foreach (var m in manifest.GetMuxedStreams()
                         .OrderByDescending(s => s.VideoResolution.Height))
            {
                results.Add(($"{m.VideoResolution.Height}p {m.Container.Name} (muxed)",
                    m.Url,
                    true));
            }

            // Video-only
            foreach (var v in manifest.GetVideoOnlyStreams()
                         .OrderByDescending(s => s.VideoResolution.Height))
            {
                results.Add(($"{v.VideoResolution.Height}p {v.Container.Name} (video only)",
                    v.Url,
                    false));
            }

            // Audio-only
            foreach (var a in manifest.GetAudioOnlyStreams()
                         .OrderByDescending(s => s.Bitrate))
            {
                results.Add(($"{a.Bitrate.KiloBitsPerSecond} kbps {a.Container.Name} (audio only)",
                    a.Url,
                    false));
            }

            return results;
        }
        // If format is muxed: download directly.
        // If video-only: pick best matching audio and merge with FFmpeg to MP4.
        public async Task<string> DownloadSelectedAsync(string url, string itag)
        {
            var manifest = await _ytClient.Videos.Streams.GetManifestAsync(url);

            // Try muxed first
            var muxed = manifest.GetMuxedStreams().FirstOrDefault(s => s.Url == url);
            var outPath = Path.Combine(Path.GetTempPath(), $"yt_{DateTime.UtcNow.Ticks}.mp4");

            if (muxed is not null)
            {
                await _ytClient.Videos.Streams.DownloadAsync(muxed, outPath);
                return outPath;
            }

            // Else assume it was a video-only selection → find that video & best audio, then merge
            var video = manifest.GetVideoOnlyStreams().First(s => s.Url == url);
            var audio = manifest.GetAudioOnlyStreams().OrderByDescending(a => a.Bitrate).First();

            var vPath = Path.Combine(Path.GetTempPath(), $"v_{DateTime.UtcNow.Ticks}.{video.Container.Name}");
            var aPath = Path.Combine(Path.GetTempPath(), $"a_{DateTime.UtcNow.Ticks}.{audio.Container.Name}");
            await _ytClient.Videos.Streams.DownloadAsync(video, vPath);
            await _ytClient.Videos.Streams.DownloadAsync(audio, aPath);

            // Merge with FFmpeg (stream copy when possible)
            var conv = FFmpeg.Conversions.New()
                .AddParameter($"-i \"{vPath}\" -i \"{aPath}\" -c copy -movflags +faststart \"{outPath}\"", ParameterPosition.PreInput);
            await conv.Start();

            try { File.Delete(vPath); } catch { }
            try { File.Delete(aPath); } catch { }

            return outPath;
        }
    }
}
