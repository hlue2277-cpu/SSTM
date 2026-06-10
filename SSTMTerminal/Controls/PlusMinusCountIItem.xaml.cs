using System;
using System.Collections.Generic;
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

namespace SSTMTerminal.Controls
{
    /// <summary>
    /// PlusMinusCountIItem.xaml 的交互逻辑
    /// </summary>
    public partial class PlusMinusCountIItem : UserControl
    {


        public bool IsPlusEnable
        {
            get { return (bool)GetValue(IsPlusEnableProperty); }
            set { SetValue(IsPlusEnableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPlusEnable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPlusEnableProperty =
            DependencyProperty.Register("IsPlusEnable", typeof(bool), typeof(PlusMinusCountIItem), new PropertyMetadata(true));

        public bool IsMinusEnable
        {
            get { return (bool)GetValue(IsMinusEnableProperty); }
            set { SetValue(IsMinusEnableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMinusEnable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMinusEnableProperty =
            DependencyProperty.Register("IsMinusEnable", typeof(bool), typeof(PlusMinusCountIItem), new PropertyMetadata(true));



        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Count.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register("Count", typeof(int), typeof(PlusMinusCountIItem), new PropertyMetadata(0,OnCountChanged));



        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(int), typeof(PlusMinusCountIItem), new PropertyMetadata(5));




        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(int), typeof(PlusMinusCountIItem), new PropertyMetadata(0));


        private static void OnCountChanged(DependencyObject dependency, DependencyPropertyChangedEventArgs e)
        {
            if(dependency is PlusMinusCountIItem item)
            {
                item.CheckCount(null);
            }
        }

        public PlusMinusCountIItem()
        {
            InitializeComponent();
            CheckCount(null);
        }

        private void plus_Click(object sender, RoutedEventArgs e)
        {
            CheckCount(true);
        }

        private void plus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CheckCount(true);
        }

        private void minus_Click(object sender, RoutedEventArgs e)
        {
            CheckCount(false);
        }

        private void minus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CheckCount(false);
        }

        private void CheckCount(bool? isPlus)
        {
            IsMinusEnable = Count > MinValue;
            IsPlusEnable = Count < MaxValue;

            if (isPlus != null)
            {
                if (isPlus.Value)
                {
                    Count++;
                }
                else
                {
                    Count--;
                }
            }

            IsMinusEnable = Count > MinValue;
            IsPlusEnable = Count < MaxValue;
        }
    }
}
