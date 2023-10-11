using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DCSInsight.Misc
{
    internal enum RangeLimitsEnum
    {
        From,
        To, 
        None
    }

    internal class TextBoxParam : TextBox
    {
        public RangeLimitsEnum RangeLimit { get; set; }
    }
}
