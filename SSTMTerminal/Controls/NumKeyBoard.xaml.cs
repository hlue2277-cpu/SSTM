using System.Windows;
using System.Windows.Controls;
using WindowsInput;
using WindowsInput.Native;

namespace SSTMTerminal.Controls
{
    /// <summary>
    /// NumKeyBoard.xaml 的交互逻辑
    /// </summary>
    public partial class NumKeyBoard : UserControl
    {
        public NumKeyBoard()
        {
            InitializeComponent();
        }

        public delegate void ClearAll();

        public event ClearAll OnClearAll;

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
            var sim = new InputSimulator();
            switch (button.Tag.ToString())
            {
                case "0":
                    sim.Keyboard.KeyDown(VirtualKeyCode.NUMPAD0)
                                .KeyUp(VirtualKeyCode.NUMPAD0);
                    break;

                case "1":
                    sim.Keyboard.KeyDown(VirtualKeyCode.NUMPAD1)
                                .KeyUp(VirtualKeyCode.NUMPAD1);
                    break;

                case "2":
                    sim.Keyboard.KeyDown(VirtualKeyCode.NUMPAD2)
                                .KeyUp(VirtualKeyCode.NUMPAD2);
                    break;

                case "3":
                    sim.Keyboard.KeyDown(VirtualKeyCode.NUMPAD3)
                                .KeyUp(VirtualKeyCode.NUMPAD3);
                    break;

                case "4":
                    sim.Keyboard.KeyDown(VirtualKeyCode.NUMPAD4)
                                .KeyUp(VirtualKeyCode.NUMPAD4);
                    break;

                case "5":
                    sim.Keyboard.KeyDown(VirtualKeyCode.NUMPAD5)
                                .KeyUp(VirtualKeyCode.NUMPAD5);
                    break;

                case "6":
                    sim.Keyboard.KeyDown(VirtualKeyCode.NUMPAD6)
                                .KeyUp(VirtualKeyCode.NUMPAD6);
                    break;

                case "7":
                    sim.Keyboard.KeyDown(VirtualKeyCode.NUMPAD7)
                                .KeyUp(VirtualKeyCode.NUMPAD7);
                    break;

                case "8":
                    sim.Keyboard.KeyDown(VirtualKeyCode.NUMPAD8)
                                .KeyUp(VirtualKeyCode.NUMPAD8);
                    break;

                case "9":
                    sim.Keyboard.KeyDown(VirtualKeyCode.NUMPAD9)
                                .KeyUp(VirtualKeyCode.NUMPAD9);
                    break;

                case "Up":
                    sim.Keyboard
                        .KeyDown(VirtualKeyCode.SHIFT)
                        .KeyDown(VirtualKeyCode.TAB)
                        .KeyUp(VirtualKeyCode.SHIFT)
                        .KeyUp(VirtualKeyCode.TAB);
                    break;

                case "Down":
                    sim.Keyboard.KeyDown(VirtualKeyCode.TAB)
                                .KeyUp(VirtualKeyCode.TAB);
                    break;

                case "Decimal":
                    sim.Keyboard.KeyDown(VirtualKeyCode.DECIMAL)
                                .Sleep(100)
                                .KeyUp(VirtualKeyCode.DECIMAL);
                    break;

                case "Back":
                    sim.Keyboard.KeyDown(VirtualKeyCode.BACK)
                                .KeyUp(VirtualKeyCode.BACK);
                    break;

                case "Enter":
                    sim.Keyboard
                        .KeyDown(VirtualKeyCode.RETURN)
                        .Sleep(100)
                        .KeyUp(VirtualKeyCode.RETURN);
                    break;

                case "Clear":
                    if (OnClearAll != null) OnClearAll();
                    break;
            }
        }
    }
}