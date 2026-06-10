using Genesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTMTerminal.Models
{
    public class UserInfoModel : NotifyObject
    {
        private string user;
        public string User
        {
            get { return user; }
            set
            {
                if (value != user)
                {
                    user = value;
                    OnPropertyChanged(() => User);
                }
            }
        }

        private string pwd;
        public string Pwd
        {
            get { return pwd; }
            set
            {
                if (pwd != value)
                {
                    pwd = value;
                    OnPropertyChanged(() => Pwd);
                }
            }
        }
    }
}
