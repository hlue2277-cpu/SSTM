using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SSTMTerminal.Controls
{
    public class IconButton: Button
    {
        public ImageBrush DisableBrush
        {
            get { return (ImageBrush)GetValue(DisableBrushProperty); }
            set { SetValue(DisableBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisableBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisableBrushProperty =
            DependencyProperty.Register("DisableBrush", typeof(ImageBrush), typeof(IconButton), new PropertyMetadata(null));
    }
}
