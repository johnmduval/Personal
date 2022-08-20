using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBase
{
    public static class StringFormatUtils
    {
        public static string MakeColumnsFixedWidth(List<string> cellValues, List<int> columnWidths)
        {
            if (cellValues.Count != columnWidths.Count())
                throw new ArgumentException("cellValues.Count is not equal to columnWidths.Count");

            var fixedWidthCols = cellValues.Select((e, i) => e.MakeFixedWidth(columnWidths[i]));

            return string.Join(" ", fixedWidthCols);
        }
    }
}
