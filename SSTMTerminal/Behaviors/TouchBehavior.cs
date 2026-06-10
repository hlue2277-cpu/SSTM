using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SSTMTerminal.Behaviors
{
    public class TouchBehavior
    {
        public static bool GetEnableTouchEffect(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableTouchEffectProperty);
        }

        public static void SetEnableTouchEffect(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableTouchEffectProperty, value);
        }

        // Using a DependencyProperty as the backing store for EnableTouchEffect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableTouchEffectProperty =
            DependencyProperty.RegisterAttached("EnableTouchEffect", typeof(bool), typeof(TouchBehavior), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnEnableTouchEffect)));


        private static void OnEnableTouchEffect(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;
            Boolean newValue;

            if (element != null)
            {
                Boolean.TryParse(e.NewValue.ToString(), out newValue);

                if (newValue)
                {
                    element.TouchDown += Element_Down;
                    element.MouseLeftButtonDown += Element_Down;
                    element.MouseLeftButtonUp += Element_Up;
                    element.MouseEnter += Element_Enter;
                    element.MouseLeave += Element_Leave;
                    element.TouchEnter += Element_Enter;
                    element.TouchUp += Element_Up;
                }
                else
                {
                    element.TouchDown -= Element_Down;
                    element.MouseLeftButtonDown -= Element_Down;
                    element.MouseLeftButtonUp -= Element_Up;
                    element.MouseEnter -= Element_Enter;
                    element.MouseLeave -= Element_Leave;
                    element.TouchEnter -= Element_Enter;
                    element.TouchUp -= Element_Up;
                }
            }
        }

        private static void Element_Leave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var element = sender as Button;
            if (element != null)
            {
                element.BorderBrush = new SolidColorBrush(Colors.Transparent);
                element.BorderThickness = new Thickness(0);
            }
        }


        private static void Element_Enter(object sender, EventArgs e)
        {
            var element = sender as Button;
            if (element != null)
            {
                element.BorderBrush = new SolidColorBrush(Colors.WhiteSmoke);
                element.BorderThickness = new Thickness(4);
            }
        }

        private static void Element_Up(object sender, EventArgs e)
        {
            var element = sender as UIElement;
            if (element != null)
            {
                element.Opacity = 1;
            }
        }

        private static void Element_Down(object sender, EventArgs e)
        {
            var element = sender as UIElement;
            if (element != null)
            {
                element.Opacity = 0.5;
            }
        }
    }
}
