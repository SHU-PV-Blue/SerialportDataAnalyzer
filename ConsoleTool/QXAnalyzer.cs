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
			if(CheckWeather(messageString))
				return true;
			return false;
		}

		public static bool CheckWeather(string message)
		{
			if (message.Contains("0020"))
			{
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
	}
}
