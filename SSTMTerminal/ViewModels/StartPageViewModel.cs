using System;
using System.Windows.Input;
using Genesis;
using Genesis.Logging;
// using Prism.Commands;        // 先注释掉，防止报错
// using Prism.Regions;         // 先注释掉

namespace SSTMTerminal.ViewModels
{
    public class StartPageViewModel : ViewModelBase
    {
        // 临时使用弱引用方式，避免直接依赖 IRegionManager（先让它编译通过）
        private readonly object _regionManager;   // 临时占位

        public ICommand StartBuyingCommand { get; private set; }

        public StartPageViewModel(ILogger logger) : base(logger)   // 先去掉 IRegionManager 参数
        {
            StartBuyingCommand = new DelegateCommand(NavigateToHome);   // 如果还是报错，后面再改
        }

        private void NavigateToHome()
        {
            try
            {
                // 临时方案：等 Loader.cs 里统一处理跳转
                // 目前先弹窗提示，后面再改成真正跳转
                System.Windows.MessageBox.Show("即将跳转到购票首页...", "提示");

                // 后面会通过 Loader.cs 来处理跳转
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex, "从启动页跳转失败！", null);
            }
        }
    }
}