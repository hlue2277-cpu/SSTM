using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SSTMTerminal.Behaviors
{
    /// <summary>
    /// 密码输入框附加行为（password字符绑定以及输入、鼠标选择等）
    /// </summary>
    public static class PasswordBoxBehavior
    {
        #region Password
        /// <summary>
        /// Get the password value.
        /// </summary>
        /// <param name="dp">PasswordProperty</param>
        /// <returns>Password value</returns>
        public static string GetPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(PasswordProperty);
        }

        /// <summary>
        /// Set the password to dependency property.
        /// </summary>
        /// <param name="dp">PasswordProperty</param>
        /// <param name="value">new password value</param>
        public static void SetPassword(DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordBoxBehavior), new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));


        public static bool GetAttach(DependencyObject dp)
        {
            return (bool)dp.GetValue(AttachProperty);
        }

        public static void SetAttach(DependencyObject dp, bool value)
        {
            dp.SetValue(AttachProperty, value);
        }

        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(PasswordBoxBehavior), new PropertyMetadata(false, Attach));

        /// <summary>
        /// Get whether password is updating or not.
        /// </summary>
        /// <param name="dp">IsUpdatingProperty</param>
        /// <returns>IsUpdating value</returns>
        private static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }

        /// <summary>
        /// Set whether password is updating or not.
        /// </summary>
        /// <param name="dp">IsUpdatingProperty</param>
        /// <param name="value">nwe is updating value</param>
        private static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }

        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordBoxBehavior));

        public static bool GetAutoSelectAll(PasswordBox passWordBox)
        {
            return (bool)passWordBox.GetValue(AutoSelectAllProperty);
        }

        public static void SetAutoSelectAll(PasswordBox passWordBox, bool value)
        {
            passWordBox.SetValue(AutoSelectAllProperty, value);
        }

        public static readonly DependencyProperty AutoSelectAllProperty =
            DependencyProperty.RegisterAttached("AutoSelectAll", typeof(bool), typeof(PasswordBoxBehavior), new FrameworkPropertyMetadata((bool)false, new PropertyChangedCallback(OnAutoSelectAllChanged)));

        #endregion

        private static void PasswordBoxOnGotFocus(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                passwordBox.SelectAll();
            }
        }

        /// <summary>
        /// It will excute when preview mouse left button down.
        /// </summary>
        /// <param name="sender">Sneder.</param>
        /// <param name="e">e.</param>
        private static void LeftButtonDownEvent(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                if (!passwordBox.IsKeyboardFocusWithin)
                {
                    // If the text box is not yet focussed, give it the focus and stop further processing of this click event.
                    passwordBox.Focus();
                    e.Handled = true;
                }
            }
        }

        private static void OnAutoSelectAllChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = d as PasswordBox;
            if (passwordBox != null)
            {
                var flag = (bool)e.NewValue;
                if (flag)
                {
                    passwordBox.GotFocus += PasswordBoxOnGotFocus;
                    passwordBox.PreviewMouseLeftButtonDown += LeftButtonDownEvent;
                }
                else
                {
                    passwordBox.GotFocus -= PasswordBoxOnGotFocus;
                    passwordBox.PreviewMouseLeftButtonDown -= LeftButtonDownEvent;
                }
            }
        }

        /// <summary>
        /// Password changed event when change password dependency property.
        /// </summary>
        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            passwordBox.PasswordChanged -= PasswordChanged;
            if (!(bool)GetIsUpdating(passwordBox))
            {
                passwordBox.Password = (string)e.NewValue;
            }
            passwordBox.PasswordChanged += PasswordChanged;
        }

        private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox == null)
                return;
            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
            }
            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
        }
    }
}
