using Printing;
using SSTMTerminal.Helpers;
using SSTMTerminal.Views;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace SSTMTerminal
{
    /// <summary>
    /// Shell.xaml 的交互逻辑
    /// </summary>
    public partial class Shell : Window
    {
        private const int WM_KEYCHAR = 0x102;
        private const int CTRL_F = 6;

        private readonly bool _isCursorEnabled = false;

        public Shell()
        {
            InitializeComponent();
            this.Loaded += Shell_Loaded;
            //Mouse.OverrideCursor = _isCursorEnabled ? null : Cursors.None;
            ComponentDispatcher.ThreadPreprocessMessage += OnThreadPreprocessMessage;
        }

        private void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            PrintManager.SetPrintContainer(printBd);
        }

        private void OnThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message == WM_KEYCHAR)
            {
                if (CTRL_F == msg.wParam.ToInt32())
                {
                    if (WindowStyle != WindowStyle.None || WindowState != WindowState.Maximized)
                    {
                        WindowStyle = WindowStyle.None;
                        WindowState = WindowState.Maximized;

                        // Hide cursor when cursor is not configured to show.
                        if (!_isCursorEnabled)
                        {
                            Mouse.OverrideCursor = Cursors.None;
                        }
                    }
                    else
                    {
                        WindowStyle = WindowStyle.ToolWindow;
                        WindowState = WindowState.Normal;
                        ResizeMode = ResizeMode.CanResize;
                        Topmost = false;

                        // Always need to show cursor in normal window mode.
                        Mouse.OverrideCursor = null;
                    }
                }
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(CenterRegionControl?.Content is IKeyDownListener keyDownListener)
            {
                keyDownListener.OnKeyDown(e);
            }
        }
    }
}