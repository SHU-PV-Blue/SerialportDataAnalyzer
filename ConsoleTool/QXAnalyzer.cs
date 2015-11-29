using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialportDataAnalyzer
{
	static class QXAnalyzer
	{
		public static bool Analy(DateTime time, List<KeyValuePair<byte, bool>> messgeQueue)
		{
			string messageString = TransferToString(messgeQueue);
			Console.WriteLine(messageString);
			if (CheckWeather(messageString))
				return true;
			return false;
		}

		public static bool CheckWeather(string message)
		{
			if (message.Contains("03030020"))
			{
				int index = message.IndexOf("03030020");
				//Console.WriteLine("0020第一次出现的位置: " + index);
				string subString = message.Substring(index, 72);			//数据字串
				byte[] DataByte = SToBa(subString);						//数据字串对应的数组
				index += 72;
				string checkSubString = message.Substring(index, 4);	//校验字串
				byte[] CheckByte = SToBa(checkSubString);				//校验字串数组

				//Console.WriteLine("字串: " + subString);
				//Console.WriteLine("校验字串: " + checkSubString);
				//for (int i = 0; i < DataByte.Length; i++)
				//	Console.Write(DataByte[i] + "  ");
				//Console.WriteLine();
				return true;
			}
			return false;
		}
		/// <summary>
		/// 将列表转为字符串用于分析
		/// </summary>
		/// <param name="messgeQueue"></param>
		/// <returns>返回的字符串, 对应十六进制</returns>
		public static string TransferToString(List<KeyValuePair<byte, bool>> messgeQueue)
		{
			string messageString = "";
			byte[] messageByte = new byte[2000];
			int j = 0;
			for (int i = 0; i < messgeQueue.Count; i++)
			{
				if (messgeQueue[i].Value == true)
					messageByte[j++] = messgeQueue[i].Key;
			}
			messageString = BaToS(messageByte, j);
			return messageString;
		}

		/// <summary>
		/// 将一个字节数组转为字符串
		/// </summary>
		/// <param name="buffer">需要转化的字节数组</param>
		/// <returns>相对应的字符串</returns>
		public static string BaToS(byte[] buffer, int n)
		{
			StringBuilder builder = new StringBuilder();
			string s = "";
			for (int i = 0; i < n; i++)
			{
				builder.Append(buffer[i].ToString("X2"));
			}
			s = builder.ToString();
			s.Trim();
			return s;
		}

		/// <summary>
		/// 将字符串转为字节数组
		/// </summary>
		/// <param name="s"></param>
		/// <returns>返回字节数组</returns>
		public static byte[] SToBa(string s)
		{
			s = s.Replace(" ", "");
			byte[] Sendbyte = new byte[s.Length / 2];
			for (int i = 0, j = 0; i < s.Length; i = i + 2, j++)
			{
				string mysubsing = s.Substring(i, 2);
				Sendbyte[j] = Convert.ToByte(mysubsing, 16);
			}

			return Sendbyte;
		}
	}
}
