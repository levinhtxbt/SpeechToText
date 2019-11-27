using System;
using NReco.VideoConverter;

namespace VideoConverter.FullNet
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var ffMpeg = new FFMpegConverter();
                ffMpeg.ConvertMedia("a.mp4",
                    "b.flac", Format.flac);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}