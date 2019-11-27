using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Enums;
using Xabe.FFmpeg.Streams;

namespace VideoConverter
{
    public static class VideoConverter
    {
        public static async Task<bool> ExtractAudio(string filePath, string outputPath, double? from = null, double? to = null)
        {
            try
            {
                IMediaInfo info = await MediaInfo.Get(filePath);

                IAudioStream audioStream = info.AudioStreams.FirstOrDefault();


                var sbSplit = new StringBuilder();

                if (from.HasValue)
                {
                    sbSplit.Append($" -ss {from}");
                }
                else
                {
                    sbSplit.Append(" -ss 0");
                }

                if (to.HasValue)
                {
                    sbSplit.Append($" -t {to}");
                }
                
                
                await Conversion.New()
                    .UseHardwareAcceleration(HardwareAccelerator.Auto, VideoCodec.H264_cuvid, VideoCodec.H264_cuvid)
                    .AddStream(audioStream)
                    .SetOutput(outputPath)
                    .AddParameter("-ac 1")
                    .AddParameter("-ar 16000")
                    .AddParameter(sbSplit.ToString())
                    .Start();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}