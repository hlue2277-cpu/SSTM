using Genesis;
using Genesis.Logging;
using System;
using System.Windows.Input;
//using Prism.Regions;          // 如果还是报错，先注释掉这行试试

namespace SSTMTerminal.ViewModels
{
    public class StartPageViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;

        public ICommand StartBuyingCommand { get; private set; }

        public StartPageViewModel(ILogger logger, IRegionManager regionManager) : base(logger)
        {
            _regionManager = regionManager;

            StartBuyingCommand = new DelegateCommand(NavigateToHome);
        }

        private void NavigateToHome()
        {
            try
            {
                _regionManager.RequestNavigate(RegionNames.CenterRegion, "HomeView");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex, "从启动页跳转到 Home 页面失败！", null);
            }
        }
    }
}