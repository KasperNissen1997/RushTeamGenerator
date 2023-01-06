namespace TeamGenerator.MVVM.ViewModels
{
    public class CreatePlayerViewModel
    {
        public string Name { get; set; }
        public string Nickname { get; set; }
        public int Rating { get; set; }

        public bool SpeaksDanish { get; set; }
        public bool SpeaksEnglish { get; set; }
    }
}
