using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace SSTMTerminal.Helpers
{
    public static class AdornerLayerHelper
    {
        public static AdornerLayer GetAdornerLayer(Visual visual)
        {
            if (visual == null)
            {
                throw new ArgumentNullException("visual");
            }
            for (Visual parentVisual = VisualTreeHelper.GetParent(visual) as Visual; parentVisual != null; parentVisual = VisualTreeHelper.GetParent(parentVisual) as Visual)
            {
                if (parentVisual is AdornerDecorator)
                {
                    return ((AdornerDecorator)parentVisual).AdornerLayer;
                }
            }

            return null;

        }
    }
}
