using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace SSTMTerminal.Controls
{
    public class NotifyUC : UserControl
    {
        private DispatcherTimer _timer = null;
        private int _countDown = 120;

        public int CountDown
        {
            get { return (int)GetValue(CountDownProperty); }
            set { SetValue(CountDownProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CountDown.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CountDownProperty =
            DependencyProperty.Register("CountDown", typeof(int), typeof(NotifyUC), new PropertyMetadata(120));

        public bool IsTimerEnable
        {
            get { return (bool)GetValue(IsTimerEnableProperty); }
            set { SetValue(IsTimerEnableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTimerEnable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTimerEnableProperty =
            DependencyProperty.Register("IsTimerEnable", typeof(bool), typeof(NotifyUC), new PropertyMetadata(false, IsTimerEnableChangedCallback));

        public ICommand TimeOutCommand
        {
            get { return (ICommand)GetValue(TimeOutCommandProperty); }
            set { SetValue(TimeOutCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeOutCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeOutCommandProperty =
            DependencyProperty.Register("TimeOutCommand", typeof(ICommand), typeof(NotifyUC));

        public ICommand LoadedCommand
        {
            get { return (ICommand)GetValue(LoadedCommandProperty); }
            set { SetValue(LoadedCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LoadedCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadedCommandProperty =
            DependencyProperty.Register("LoadedCommand", typeof(ICommand), typeof(UserControl));


        public NotifyUC()
        {
            this.Loaded += NotifyUC_Loaded;
        }

        private static void IsTimerEnableChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is NotifyUC nuc && e.NewValue is bool newValue)
            {
                if (newValue)
                {
                    if (nuc.IsLoaded)
                    {
                        nuc.InitTimerAndStart();
                    }
                    else
                    {
                        nuc.IsNeedWaitForLoadedToStartTimer = true;
                    }
                }
                else
                {
                    nuc.StopTimer();
                }
            }
        }

        public bool IsNeedWaitForLoadedToStartTimer = false;

        private void NotifyUC_Loaded(object sender, RoutedEventArgs e)
        {
            _countDown = CountDown;
            if (LoadedCommand != null)
            {
                LoadedCommand.Execute(null);
            }

            if (IsNeedWaitForLoadedToStartTimer)
            {
                InitTimerAndStart();
            }
        }

        private void InitTimerAndStart()
        {
            CountDown = _countDown;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick -= Timer_Tick;
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void StopTimer()
        {
            if (_timer != null)
            {
                _timer.Tick -= Timer_Tick;
				_timer.Stop();
                _timer = null;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            CountDown--;
            if (CountDown <= 0)
            {
                StopTimer();

                //timeout
                if (TimeOutCommand != null)
                {
                    TimeOutCommand.Execute(true);
                }
            }
        }
    }
}
