using Genesis;
using Genesis.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SSTMTerminal.ViewModels
{
    public interface IMsgWithoutCountDownViewModel : IViewModel
    {
        string Message { get; set; }
        void SetValue(Visibility titleVisibility, string titleImageSource,
          string contentImageSource, string msg,
          Visibility backButtonVisibility);

        ICommand BackToHomeCommand { get; set; }

    }

    public class MsgWithoutCountDownViewModel : ViewModelBase, IMsgWithoutCountDownViewModel
    {
        public MsgWithoutCountDownViewModel(ILogger logger)
            : base(logger)
        {
            Message = string.Empty;
        }

        private string message;

        public string Message
        {
            get { return message; }
            set
            {
                if (value != message)
                {
                    message = value;
                    OnPropertyChanged(() => Message);
                }
            }
        }

        public ICommand BackToHomeCommand { get; set; }

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

        public void SetValue(Visibility titleVisibility, string titleImageSource,
            string contentImageSource, string msg,
            Visibility backButtonVisibility)
        {
            this.TitleVisibility = titleVisibility;
            this.TitleImageSource = titleImageSource;
            this.ContentImageSource = contentImageSource;
            this.Message = msg;
            this.BackButtonVisibility = backButtonVisibility;
        }
    }
}