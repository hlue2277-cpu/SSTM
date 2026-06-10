using SSTMTerminal.Controls;
using SSTMTerminal.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace SSTMTerminal.Views
{
    /// <summary>
    /// ErrorView.xaml 的交互逻辑
    /// </summary>
    public partial class ExhibitionPaymentCountDownView : NotifyUC, IKeyDownListener
    {
        public ExhibitionPaymentCountDownView()
        {
            InitializeComponent();
        }

        private static KeyConverter KeyConverter = new KeyConverter();


        private StringBuilder scannedData = new StringBuilder(200);

        void IKeyDownListener.OnKeyDown(KeyEventArgs e)
        {
            var vm = this.DataContext as ExhibitionPaymentCountDownViewModel;
            if (vm != null && vm.ReadyToScan)
            {
                // 扫描事件已经结束
                if (e.Key == Key.Enter)
                {
                    // scan is over.
                    var finalString = scannedData.ToString();
                    // pass into view model
                    vm.ScanDataReceivedEvent?.Invoke(vm, new BarCodeScanDataEventArgs() { RequestGuid = vm.RequestGuid, ScannedData = finalString });
                }
                else
                {
                    var inputChar = KeyConverter.ConvertToString(e.Key);
                    scannedData.Append(inputChar);
                }
            }
        }
    }
}
