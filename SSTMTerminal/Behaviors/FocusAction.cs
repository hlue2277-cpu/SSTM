using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace SSTMTerminal.Behaviors
{
    /// <summary>
    /// This class is inherit from TargetedTriggerAction to focus target element.
    /// </summary>
    public class FocusAction : TargetedTriggerAction<DependencyObject>
    {
        /// <summary>
        /// if invoked when target is not yet specified, will focus once the target is set.
        /// </summary>
        /// <param name="parameter"></param>
        protected override void Invoke(object parameter)
        {
            if (Target is UIElement && !((UIElement)Target).IsKeyboardFocusWithin)
            {
                UIElement element = (UIElement)Target;

                Action a = () =>
                {
                    element.Focus();
                    Keyboard.Focus(element);
                };

                // When the main thread is excuted completly then set the focus to this UIElement. So the focus will not be effected by other control's event.
                element.Dispatcher.BeginInvoke(a, DispatcherPriority.Background);
            }
        }
    }
}
