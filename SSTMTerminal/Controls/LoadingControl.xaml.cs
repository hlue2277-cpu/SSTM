using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SSTMTerminal.Controls
{
    /// <summary>
    /// LoadingControl.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingControl : UserControl
    {
        public LoadingControl()
        {
            InitializeComponent();
            Loaded += LoadingControl_Loaded;
        }
        void LoadingControl_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.04);
            timer.Tick += new EventHandler(OnTimerTick);
            timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            foreach (var item in PathCollection.Children)
            {
                Path itemPath = (Path)item;

                double itemCanvasLeft = Canvas.GetLeft(itemPath);

                if (itemCanvasLeft == 180)
                {
                    Canvas.SetLeft(itemPath, 0);
                }
                else
                {
                    Canvas.SetLeft(itemPath, itemCanvasLeft + 10);
                }
            }
        }

        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(LoadingControl), new PropertyMetadata(false));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(LoadingControl), new PropertyMetadata(null));
    }
}
