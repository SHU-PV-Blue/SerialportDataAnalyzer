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

            string regex = @"AA0012091100(A-Za-z0-9_){34}CC33C33C";
            Regex re = new Regex(regex);
            if(re.IsMatch(byteStr))
            {
                Match byteMatch = re.Match(byteStr);
                string reasultStr = byteMatch.ToString();
                int tep = Convert.ToInt32(Inverse(reasultStr.Substring(2, 4)), 16);
                double Vo = Convert.ToInt32(Inverse(reasultStr.Substring(10, 4)), 16)/10;
                double Is = Convert.ToInt32(Inverse(reasultStr.Substring(14, 4)), 16)/100;
                double Vm = Convert.ToInt32(Inverse(reasultStr.Substring(18, 4)), 16)/10;
                double Im = Convert.ToInt32(Inverse(reasultStr.Substring(22, 4)), 16)/100;
                double Pm = Convert.ToInt32(Inverse(reasultStr.Substring(26, 8)), 16)/10;

            }

            return true;
		}

        public static string Inverse(string str)
        {
            int length = str.Length;
            string newStr="";
            for(int i=0; i<length; i+=2)
            {
                newStr = str[i] + str[i+1] + newStr;
            }
            return newStr;
        }
	}
}
