using System;
using System.Windows.Input;
using Genesis;
using Genesis.Logging;

namespace SSTMTerminal.ViewModels
{
    public interface IStartPageViewModel : IViewModel
    {
        ICommand StartBuyingCommand { get; }
    }

    public class StartPageViewModel : ViewModelBase, IStartPageViewModel
    {
        public ICommand StartBuyingCommand { get; private set; }

        public StartPageViewModel(ILogger logger) : base(logger)
        {
            StartBuyingCommand = new SimpleCommand(NavigateToHome);   // 使用简单实现
        }

        private void NavigateToHome()
        {
            System.Windows.MessageBox.Show("开始购票\n\n即将进入购票首页...", "提示");
        }
    }

    // 简单命令实现（放在同一个文件底部）
    public class SimpleCommand : ICommand
    {
        private readonly Action _execute;
        public SimpleCommand(Action execute) => _execute = execute;

        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _execute?.Invoke();
    }
}