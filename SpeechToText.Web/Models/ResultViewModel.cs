namespace SpeechToText.Web.Models
{
    public class ResultViewModel
    {
        public string Transcript { get; set; }

        public ResultViewModel(string transcript)
        {
            Transcript = transcript;
        }

        public ResultViewModel()
        {
            
        }
    }
}