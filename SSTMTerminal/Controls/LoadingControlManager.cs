using Genesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTMTerminal.Controls
{
    /// <summary>
    /// 加载控件管理类
    /// </summary>
    public class LoadingControlManager : NotifyObject
    {
        private static LoadingControlManager _instance = null;

        private bool _isBusy = false;
        private string _message = string.Empty;

        /// <summary>
        /// Gets or sets is busy.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                if (value != _isBusy)
                {
                    _isBusy = value;
                    OnPropertyChanged(() => IsBusy);
                }
            }
        }

        /// <summary>
        /// Gets or sets the message displayed when is busy.
        /// </summary>
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (value != _message)
                {
                    _message = value;
                    OnPropertyChanged(() => Message);
                }
            }
        }

        /// <summary>
        /// Gets the current language resource.
        /// </summary>
        public static LoadingControlManager Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new LoadingControlManager();
                }

                return _instance;
            }
        }
    }
}
