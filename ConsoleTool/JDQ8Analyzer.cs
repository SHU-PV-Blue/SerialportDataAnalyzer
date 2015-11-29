using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialportDataAnalyzer
{
	static class JDQ8Analyzer
	{
        private const string returnStr = "020F00000008543E";
		public static bool Analy(DateTime time, List<KeyValuePair<byte, bool>> messageQueue)
		{
            //!!!!!
            byte[] bytes = messageQueue.Select(b => b.Key).ToArray();

            String message = Transfer.BaToS(bytes);
            int index = -1;
            while ((index = message.IndexOf(returnStr, index + 1)) != -1)
            {
                for (int i = index / 2; i < (index + returnStr.Length) / 2; i++)
                    messageQueue[i] = new KeyValuePair<byte, bool>(messageQueue[i].Key, false);
            }

            return true;
		}
	}
}
