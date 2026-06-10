using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SSTMTerminal.Behaviors
{
    /// <summary>
    /// 设定按钮多次点击之间的限制时间
    /// </summary>
    public class ButtonBaseClickBehavior
    {
        private static readonly MethodInfo _setIsPressedMethod;
        private static readonly object[] _trueBox = new object[] { true };
        private static readonly object[] _falseBox = new object[] { false };

        private const int MAX_MILLISECONDS = 800;

        private static int _buttonHashCode = 0;
        private static DateTime? _previousMouseDownTime = null;
        private static DateTime? _previousMouseUpTime = null;

        static ButtonBaseClickBehavior()
        {
            _setIsPressedMethod = typeof(ButtonBase).GetMethod("SetIsPressed", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static int GetIgnoreMilliseconds(DependencyObject obj)
        {
            return (int)obj.GetValue(IgnoreMillisecondsProperty);
        }

        public static void SetIgnoreMilliseconds(DependencyObject obj, int value)
        {
            obj.SetValue(IgnoreMillisecondsProperty, value);
        }

        // Using a DependencyProperty as the backing store for IgnoreMilliseconds.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IgnoreMillisecondsProperty =
            DependencyProperty.RegisterAttached("IgnoreMilliseconds", typeof(int), typeof(ButtonBaseClickBehavior), new PropertyMetadata(MAX_MILLISECONDS));

        public static DependencyProperty IsDoubleClickEnabledProperty
            = DependencyProperty.RegisterAttached("IsDoubleClickEnabled", typeof(bool), typeof(ButtonBaseClickBehavior),
            new FrameworkPropertyMetadata(true, IsDoubleClickEnabledPropertyChangedCallback));

        public static bool GetIsDoubleClickEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDoubleClickEnabledProperty);
        }

        public static void SetIsDoubleClickEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDoubleClickEnabledProperty, value);
        }

        private static void IsDoubleClickEnabledPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is bool) || (bool)e.NewValue)
            {
                return;
            }

            var buttonBase = d as ButtonBase;

            if (buttonBase != null)
            {
                buttonBase.Unloaded -= OnButtonBaseUnloaded;
                buttonBase.Unloaded += OnButtonBaseUnloaded;

                buttonBase.PreviewMouseUp -= OnPreviewMouseUp;
                buttonBase.PreviewMouseUp += OnPreviewMouseUp;
                buttonBase.PreviewMouseDown -= OnPreviewMouseDown;
                buttonBase.PreviewMouseDown += OnPreviewMouseDown;

                // 
                // In Touch screen, the MouseDown event will only happen when TouchUp event fires,
                // ButtonBase.IsPressed is based on MouseDown event and most of the button highlight effect is depending on this property.
                // This is the workaround to set the IsPressed according to Touch event.
                // 
                buttonBase.TouchDown += OnButtonBaseTouchDown;
                buttonBase.TouchUp += OnButtonBaseTouchUp;
                buttonBase.LostTouchCapture += OnButtonBaseLostTouchCapture;
            }
        }

        /// <summary>
        /// Registers this method for PreviewMouseUp event of ButtonBase, check the time difference between two event,
        /// if the time difference is very small, ignore the event.
        /// </summary>
        /// <param name="sender">A <c>ButtonBase</c>, which trigger this event.</param>
        /// <param name="e">A <c>RoutedEventArgs</c>.</param>
        private static void OnPreviewMouseUp(object sender, RoutedEventArgs e)
        {
            var buttonBase = sender as ButtonBase;

            if (_buttonHashCode == buttonBase.GetHashCode() && _previousMouseDownTime.HasValue && _previousMouseUpTime.HasValue)
            {
                if ((_previousMouseDownTime.Value - _previousMouseUpTime.Value).TotalMilliseconds <= GetIgnoreMilliseconds(buttonBase))
                {
                    buttonBase.ReleaseMouseCapture();
                    e.Handled = true;
                }
            }
            else
            {
                _buttonHashCode = buttonBase.GetHashCode();
            }

            _previousMouseUpTime = DateTime.Now;
        }

        private static void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var buttonBase = sender as ButtonBase;

            if (buttonBase != null && e.ClickCount > 1)
            {
                buttonBase.ReleaseMouseCapture();
                e.Handled = true;
            }

            _previousMouseDownTime = DateTime.Now;
        }

        private static void OnButtonBaseTouchDown(object sender, TouchEventArgs e)
        {
            _setIsPressedMethod.Invoke(sender, _trueBox);
        }

        private static void OnButtonBaseTouchUp(object sender, TouchEventArgs e)
        {
            var buttonBase = sender as ButtonBase;

            buttonBase.ReleaseTouchCapture(e.TouchDevice);
            _setIsPressedMethod.Invoke(sender, _falseBox);
        }

        private static void OnButtonBaseLostTouchCapture(object sender, TouchEventArgs e)
        {
            _setIsPressedMethod.Invoke(sender, _falseBox);
        }

        private static void OnButtonBaseUnloaded(object sender, RoutedEventArgs e)
        {
            var buttonBase = sender as ButtonBase;

            if (buttonBase != null)
            {
                buttonBase.Unloaded -= OnButtonBaseUnloaded;
                buttonBase.PreviewMouseUp -= OnPreviewMouseUp;
                buttonBase.PreviewMouseDown -= OnPreviewMouseDown;
            }
        }
    }
}
