using Genesis;
using Genesis.Logging;
using System.Windows;
using System.Windows.Input;

namespace SSTMTerminal.ViewModels
{
    public interface IMsgViewModel : IViewModel
    {
        void SetValue(Visibility titleVisibility, string titleImageSource, 
            string contentImageSource, string msg, 
            Visibility backButtonVisibility, int countDown,
            IViewModel owner = null);

        ICommand BackToHomeCommand { get; set; }
        bool IsTimerEnable { get; set; }
        IViewModel Owner { get; set; }
        void Reset();
    }

    public class MsgViewModel : ViewModelBase, IMsgViewModel
    {
        public void Reset()
        {
            IsTimerEnable = false;
            Owner = null;
            CountDown = 0;
        }

        private bool isTimerEnable;
        public bool IsTimerEnable
        {
            get { return isTimerEnable; }
            set
            {
                if (isTimerEnable != value)
                {
                    isTimerEnable = value;
                    OnPropertyChanged(() => IsTimerEnable);
                }
            }
        }

        public ICommand BackToHomeCommand { get; set; }

        public IViewModel Owner { get; set; }

        private Visibility backButtonVisibility;

        public Visibility BackButtonVisibility
        {
            get { return backButtonVisibility; }
            set
            {
                if (value != backButtonVisibility)
                {
                    backButtonVisibility = value;
                    OnPropertyChanged(() => BackButtonVisibility);
                }
            }
        }

        private Visibility titleVisibility;

        public Visibility TitleVisibility
        {
            get { return titleVisibility; }
            set
            {
                if (value != titleVisibility)
                {
                    titleVisibility = value;
                    OnPropertyChanged(() => TitleVisibility);
                }
            }
        }

        private string titleImageSource;

        public string TitleImageSource
        {
            get { return titleImageSource; }
            set
            {
                if (value != titleImageSource)
                {
                    titleImageSource = value;
                    OnPropertyChanged(() => TitleImageSource);
                }
            }
        }

        private string contentImageSource;

        public string ContentImageSource
        {
            get { return contentImageSource; }
            set
            {
                if (value != contentImageSource)
                {
                    contentImageSource = value;
                    OnPropertyChanged(() => ContentImageSource);
                }
            }
        }

        private string msg;

        public string Message
        {
            get { return msg; }
            set
            {
                if (value != msg)
                {
                    msg = value;
                    OnPropertyChanged(() => Message);
                }
            }
        }

        private int countDown;
        public int CountDown
        {
            get { return countDown; }
            set
            {
                if (value != countDown)
                {
                    countDown = value;
                    OnPropertyChanged(() => CountDown);
                }
            }
        }


        public void SetValue(Visibility titleVisibility, string titleImageSource,
            string contentImageSource, string msg,
            Visibility backButtonVisibility, int countDown,
            IViewModel owner = null)
		{
            this.TitleVisibility = titleVisibility;
            this.TitleImageSource = titleImageSource;
            this.ContentImageSource = contentImageSource;
            this.Message = msg;
            this.BackButtonVisibility = backButtonVisibility;
            this.CountDown = countDown;
            this.Owner = owner;
        }
    }
}