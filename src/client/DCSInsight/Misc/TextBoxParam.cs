using System.Windows.Controls;

namespace DCSInsight.Misc
{
    internal enum RangeLimitsEnum
    {
        From,
        To
    }

    internal class TextBoxParam : TextBox
    {
        public RangeLimitsEnum RangeLimit { get; set; }
    }
}
