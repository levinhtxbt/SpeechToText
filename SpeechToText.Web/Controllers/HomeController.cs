using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Type;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpeechToText.Web.Domain;
using SpeechToText.Web.Models;
using SpeechToText.Web.Services;

namespace SpeechToText.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;

        public HomeController(ILogger<HomeController> logger,
            IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public IActionResult Index()
        {
            var data = TempData.Get<ResultViewModel>("TranscriptResult") ?? new ResultViewModel();
//            data.Transcript = "this is my data";
            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost()]
        public async Task<IActionResult> LongAudio(string gcsUri)
        {
            var result = Recognize.AsyncRecognizeGcs(gcsUri);
            var viewModel = new ResultViewModel(result);
//            var viewModel = new ResultViewModel();
            TempData.Put("TranscriptResult", viewModel);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost()]
        public async Task<IActionResult> ShortAudio(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);
            var filePaths = new List<string>();
            var strResult = new StringBuilder();
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var filePath = Path.Combine(uploads, file.FileName);
                    filePaths.Add(filePath);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }

//            foreach (var filePath in filePaths)
//                strResult.Append(Recognize.SyncRecognize(filePath));

            var viewModel = new ResultViewModel(strResult.ToString());
            TempData.Put("TranscriptResult", viewModel);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost()]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> FromVideo(IFormFile file)
        {
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            var outputPaths = new List<string>();

            List<RecognizeResult> results = new List<RecognizeResult>();
            if (file.Length > 0)
            {
                var filePath = Path.Combine(uploads, file.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                _logger.LogDebug($"Get Duration ..... ");
                // get duration of video
                var duration = VideoHelper.GetDuration(filePath);
                var currentTime = new TimeSpan();

                _logger.LogDebug($"Get Duration: {duration?.ToString("G")}");

                _logger.LogDebug("Start split and extract audio");
                // split and extract audio
                while (currentTime < duration)
                {
                    var outputFile = Path.Combine(uploads,
                        $"{file.FileName}-{DateTime.Now:ddMMyyyyHHmmss}-{outputPaths.Count + 1}.wav");

                    await VideoConverter.VideoConverter.ExtractAudio(filePath, outputFile,
                        currentTime.TotalSeconds,
                        59);

                    _logger.LogDebug($"Add {outputFile} to output list");
                    outputPaths.Add(outputFile);

                    currentTime = currentTime.Add(new TimeSpan(0, 0, 59));
                }

                _logger.LogDebug("Start Recognize text");
                // upload to GCS or call directly from local
                var tasks = from o in outputPaths select RecognizeAsync(o, outputPaths.IndexOf(o) + 1);
//                tasks.Add(RecognizeAsync(outputFile, outputPaths.Count + 1));
                var excutedTask = tasks.ToList();
                _logger.LogDebug($"Fetch list task");
                while (excutedTask.Count > 0)
                {
                    _logger.LogDebug($"Number of task in list {excutedTask.Count}");
                    // Identify the first task that completes.
                    Task<RecognizeResult> firstFinishedTask = await Task.WhenAny(excutedTask);

                    // ***Remove the selected task from the list so that you don't
                    // process it more than once.
                    excutedTask.Remove(firstFinishedTask);

                    // Await the completed task.
                    results.Add(await firstFinishedTask);
                }


                // convert to audio
//                var outputFile = Path.Combine(uploads, $"{file.FileName}-{DateTime.Now:ddMMyyyyHHmmss}.wav");
//                filePaths.Add(outputFile);
//                await VideoConverter.VideoConverter.ExtractAudio(filePath, outputFile);
//
//                // upload to gcs 
//                var gsPath = GoogleCloudStorage.UploadFile(GoogleConstant.BUCKET_NAME, outputFile,
//                    Path.GetFileName(outputFile));
//                filePaths.Add(gsPath);
// 
//                var result = Recognize.AsyncRecognizeGcs(gsPath);
                var viewModel =
                    new ResultViewModel(string.Join('\n', results.OrderBy(x => x.Index).Select(x => x.Content)));
                TempData.Put("TranscriptResult", viewModel);
            }

            return RedirectToAction("Index", "Home");
        }

        private Task<RecognizeResult> RecognizeAsync(string filePath, int order)
        {
            return Task.Run(() =>
            {
                _logger.LogDebug($"Start new thread for {filePath}");
                var r = Recognize.SyncRecognize(filePath);
                _logger.LogDebug($"End new thread for {filePath}");
                return new RecognizeResult
                {
                    Index = order,
                    Content = r
                };
            });
        }

        
        private Task<RecognizeResult> RecognizeAsync2(string filePath, int order)
        {
            _logger.LogDebug($"Start new thread for {filePath}");
            var r = Recognize.SyncRecognize(filePath);
            _logger.LogDebug($"End new thread for {filePath}");
            return Task.FromResult(new RecognizeResult
            {
                Index = order,
                Content = r
            });
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }

    public class RecognizeResult
    {
        public int Index { get; set; }
        public string Content { get; set; }

        public TimeSpan StartTime { get; set; }
        
        public TimeSpan EndTime { get; set; }
    }
}