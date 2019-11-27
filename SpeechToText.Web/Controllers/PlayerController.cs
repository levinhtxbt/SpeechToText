using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpeechToText.Web.Domain;
using SpeechToText.Web.Models;
using SpeechToText.Web.Services;

namespace SpeechToText.Web.Controllers
{
    public class PlayerController : Controller
    {
        private readonly ILogger<PlayerController> _logger;
        private readonly IWebHostEnvironment _environment;

        public PlayerController(ILogger<PlayerController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _environment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = new PlayerViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Index(IFormFile file)
        {
            if (file.Length <= 0)
                return View(new PlayerViewModel());

            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            List<RecognizeResult> results = new List<RecognizeResult>();
            PlayerViewModel viewModel = null;
            var filePath = Path.Combine(uploads, file.FileName);
            var mediaUrl = $"uploads/{file.FileName}";

            using (var fileStream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(fileStream);

            _logger.LogDebug($"Get Duration ..... ");
            var duration = VideoHelper.GetDuration(filePath);
            var currentTime = new TimeSpan();
            _logger.LogDebug($"Duration: {duration?.ToString("G")}");
            _logger.LogDebug("Start split and extract audio");

            List<Task<RecognizeResult>> tasks = new List<Task<RecognizeResult>>();
            
            // split and extract audio
            var index = 1;
            while (currentTime < duration)
            {
                var outputFile = Path.Combine(uploads,
                    $"{file.FileName}-{DateTime.Now:ddMMyyyyHHmmss}-{index}.wav");

                await VideoConverter.VideoConverter.ExtractAudio(filePath, outputFile,
                    currentTime.TotalSeconds,
                    30);
                _logger.LogDebug($"Add {outputFile} to output list");
                
                tasks.Add(RecognizeAsync(outputFile, index, currentTime.TotalSeconds,
                    currentTime.TotalSeconds + 30));
                
                currentTime = currentTime.Add(new TimeSpan(0, 0, 30));
                index++;
            }

            _logger.LogDebug("Start Recognize text");
            // upload to GCS or call directly from local
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

            viewModel = new PlayerViewModel(
                results.Select(x => new TranscriptViewModel(x)).ToList(),
                mediaUrl);

            return View(viewModel);
        }
 
        private Task<RecognizeResult> RecognizeAsync(string filePath, int index, double startTime, double endTime)
        {
            return Task.Run(() =>
            {
                _logger.LogDebug($"Start new thread for {filePath}");
                var r = Recognize.SyncRecognize(filePath);
                _logger.LogDebug($"End new thread for {filePath}");

                return new RecognizeResult
                {
                    Index = index,
                    Content = r,
                    StartTime = TimeSpan.FromSeconds(startTime),
                    EndTime = TimeSpan.FromSeconds(endTime)
                };
            });
        }
    }
}