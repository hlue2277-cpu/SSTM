using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace SSTMTerminal.Behaviors
{
    /// <summary>
    /// This class is like InvokeCommandAction inherit from TriggerAction,which override Invoke method.
    /// The invoke method will Execute command dependency property and pass EventArgs as Parameter.
    /// </summary>
    public class InvokeDelegateCommandAction : TriggerAction<DependencyObject>
    {
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(InvokeDelegateCommandAction), null);

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(InvokeDelegateCommandAction), null);

        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return this.GetValue(CommandParameterProperty); }
            set { this.SetValue(CommandParameterProperty, value); }
        }

        protected override void Invoke(object parameter)
        {
            this.CommandParameter = parameter;

            if (this.AssociatedObject != null)
            {
                if ((Command != null) && Command.CanExecute(this.CommandParameter))
                {
                    Command.Execute(this.CommandParameter);
                }
            }
        }
    }
}
