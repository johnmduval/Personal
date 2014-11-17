using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortBalance
{
	class StringUtils
	{
		public static string MakeFixedWidth(string input, int width, bool padRight = true)
		{
			int startIndex = System.Math.Max(0, input.Length - width);
			string fixedWidth = input.Substring(startIndex);
			if (padRight)
				return fixedWidth.PadRight(width);
			return fixedWidth.PadLeft(width);
		}

	}
}
