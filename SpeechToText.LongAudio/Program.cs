using System;
using Google.Cloud.Speech.V1;

namespace SpeechToText.LongAudio
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

//            var fileName = "gs://speedtotext2019/web5ngay.wav";
//            var startime = DateTime.Now;
//            Recognize.AsyncRecognizeGcs(fileName);
//            var time = DateTime.Now - startime;
//            Console.WriteLine("Executed time: " + time.TotalSeconds);

            //GoogleCloudStorage.UploadFileToGCS("");


            Console.ReadKey();


            //       return (int)Parser.Default.ParseArguments<
//                SyncOptions, AsyncOptions,
//                StreamingOptions, ListenOptions, ListenInfiniteOptions,
//                RecOptions, SyncOptionsWithCreds,
//                OptionsWithContext
//                >(args).MapResult(
//                (SyncOptions opts) => IsStorageUri(opts.FilePath) ?
//                    SyncRecognizeGcs(opts.FilePath) : opts.EnableWordTimeOffsets ?
//                    SyncRecognizeWords(opts.FilePath) : opts.EnableAutomaticPunctuation ?
//                    SyncRecognizePunctuation(opts.FilePath) : (opts.SelectModel != null) ?
//                    SyncRecognizeModelSelection(opts.FilePath, opts.SelectModel) : opts.UseEnhancedModel ?
//                    SyncRecognizeEnhancedModel(opts.FilePath) : (opts.NumberOfChannels > 1) ?
//                    SyncRecognizeMultipleChannels(opts.FilePath, opts.NumberOfChannels) : SyncRecognize(opts.FilePath),
//                (AsyncOptions opts) => IsStorageUri(opts.FilePath) ?
//                    (opts.EnableWordTimeOffsets ? AsyncRecognizeGcsWords(opts.FilePath)
//                    : AsyncRecognizeGcs(opts.FilePath))
//                    : LongRunningRecognize(opts.FilePath),
//                (StreamingOptions opts) => StreamingRecognizeAsync(opts.FilePath).Result,
//                (ListenOptions opts) => StreamingMicRecognizeAsync(opts.Seconds).Result,
//                (ListenInfiniteOptions opts) => InfiniteStreaming.RecognizeAsync().Result,
//                (RecOptions opts) => Rec(opts.FilePath, opts.BitRate, opts.Encoding),
//                (SyncOptionsWithCreds opts) => SyncRecognizeWithCredentials(
//                    opts.FilePath, opts.CredentialsFilePath),
//                (OptionsWithContext opts) => RecognizeWithContext(opts.FilePath, ReadPhrases()),
//                errs => 1);
        }
    }
}