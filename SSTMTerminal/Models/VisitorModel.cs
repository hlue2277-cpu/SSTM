using Genesis;
using SSTMTerminal.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SSTMTerminal.Models
{
    public class VisitorModel
    {
        public string ID {  get; set; }
        public string Name {  get; set; }

        // idcard, passort, residence
        public string CertType {  get; set; }

        public string VisitorImagePath => ImagePath.ItemRectangle;
    }
}
