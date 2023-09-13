using Models;

namespace ViewModels
{
    public class NewsletterViewModel
    {
        public string subject { get; set; }
        public string head { get; set; }
        public string body { get; set; }
        public string img { get; set; }
        public string email { get; set; }
        public Restaurant Restaurant { get; set; }
    }
}