using System;
using System.Collections.Generic;
using System.Threading;
using SpeechToText.Web.Controllers;

namespace SpeechToText.Web.Models
{
    public class PlayerViewModel
    {
        public List<TranscriptViewModel> Transcripts { get; set; }
            = new List<TranscriptViewModel>();

        public string MediaUrl { get; set; }

        public PlayerViewModel(List<TranscriptViewModel> transcripts, string mediaUrl)
        {
            Transcripts = transcripts;
            MediaUrl = mediaUrl;
        }

        public PlayerViewModel()
        {
            
        }
    }

    public class TranscriptViewModel
    {
        public int Index { get; set; }

        public string Content { get; set; }

        public TimeSpan StartTime { get; set; }
        
        public TimeSpan EndTime { get; set; }

        public TranscriptViewModel()
        {
            
        }

        public TranscriptViewModel(RecognizeResult m)
        {
            Index = m.Index;
            Content = m.Content;
            StartTime = m.StartTime;
            EndTime = m.EndTime;
        }
    }
}