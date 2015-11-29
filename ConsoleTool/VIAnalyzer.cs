using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SerialportDataAnalyzer
{
	static class VIAnalyzer
	{
		public static bool Analy(DateTime time, List<KeyValuePair<byte, bool>> messgeQueue)
		{
            byte[] byteArray = messgeQueue.Select(b => b.Key).ToArray();
            string byteStr = System.Text.Encoding.Default.GetString(byteArray);


            return true;
		}
	}
}
