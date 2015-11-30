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

            string eigenRegex = @"AA0012091100([A-Za-z0-9_]{34})CC33C33C";
            Regex eigenRe = new Regex(eigenRegex);

            string iRegex = @"AA0012019001([A-Za-z0-9_]{800})CC33C33C";
            Regex iRe = new Regex(iRegex);

            string vRegex = @"AA0012029090([A-Za-z0-9_]{800})CC33C33C";
            Regex vRe = new Regex(vRegex);

            if (eigenRe.IsMatch(byteStr))
            {
                Match byteMatch = eigenRe.Match(byteStr);
                string reasultStr = byteMatch.Groups[1].Value;
                int Tep = Convert.ToInt32(Inverse(reasultStr.Substring(2, 4)), 16);
                double Vo = Convert.ToInt32(Inverse(reasultStr.Substring(10, 4)), 16) / 10.0;
                double Is = Convert.ToInt32(Inverse(reasultStr.Substring(14, 4)), 16) / 100.0;
                double Vm = Convert.ToInt32(Inverse(reasultStr.Substring(18, 4)), 16) / 10.0;
                double Im = Convert.ToInt32(Inverse(reasultStr.Substring(22, 4)), 16) / 100.0;
                double Pm = Convert.ToInt64(Inverse(reasultStr.Substring(26, 8)), 16) / 10.0;
            }

            if (iRe.IsMatch(byteStr))
            {
                double[] iData = new double[200];
                Match byteMatch = iRe.Match(byteStr);
                string reasultStr = byteMatch.Groups[1].Value;
                for (int i = 0; i < 800; i += 4)
                {
                    iData[i / 4] = Convert.ToInt32(Inverse(reasultStr.Substring(i, 4)), 16) / 100.0;
                }
            }

            if (vRe.IsMatch(byteStr))
            {
                double[] vData = new double[200];
                Match byteMatch = vRe.Match(byteStr);
                string reasultStr = byteMatch.Groups[1].Value;
                for (int i = 0; i < 800; i += 4)
                {
                    vData[i / 4] = Convert.ToInt32(Inverse(reasultStr.Substring(i, 4)), 16) / 10.0;
                }
            }

            return true;
        }

        public static string Inverse(string str)
        {
            int length = str.Length;
            string newStr = "";
            for (int i = 0; i < length; i += 2)
            {
                newStr = str.Substring(i, 2) + newStr;
            }
            return newStr;
        }
    }
}
