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
    /// This class is inherit from TargetedTriggerAction to move focus from current element to next.
    /// </summary>
    public class MoveNext : TargetedTriggerAction<DependencyObject>
    {
        /// <summary>
        /// if invoked when target is not yet specified, will focused to next of current one.
        /// </summary>
        protected override void Invoke(object parameter)
        {
            UIElement element = null;

            if (string.IsNullOrEmpty(TargetName))
            {
                element = (UIElement)Keyboard.FocusedElement;
            }
            else
            {
                element = (UIElement)Target;
            }

            if (element != null)
            {
                Action a = () =>
                {
                    element.Focus();
                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                    element.MoveFocus(request);
                };

                // When the main thread is excuted completly then set the focus to this UIElement. So the focus will not be effected by other control's event.
                element.Dispatcher.BeginInvoke(a, DispatcherPriority.Background);
            }
        }
    }
}
