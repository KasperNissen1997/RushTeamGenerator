using System.ComponentModel;

namespace TeamGenerator.MVVM.ViewModels
{
    public class CreatePlayerViewModel : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Nickname { get; set; }
        public int Rating { get; set; } = 1;

        private bool _speaksDanish;
        public bool SpeaksDanish
        {
            get
            {
                return _speaksDanish;
            }

            set
            {
                _speaksDanish = value;
                OnPropertyChanged(nameof(SpeaksAnyLanguage));
            }
        }
        private bool _speaksEnglish;
        public bool SpeaksEnglish
        {
            get
            {
                return _speaksEnglish;
            }

            set
            {
                _speaksEnglish = value;
                OnPropertyChanged(nameof(SpeaksAnyLanguage));
            }
        }

        public bool SpeaksAnyLanguage
        {
            get
            {
                return SpeaksDanish || SpeaksEnglish;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged is not null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
