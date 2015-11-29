using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialportDataAnalyzer
{
	static class QXAnalyzer
	{
		private static int byteLength = 38;		//如果匹配,则有效字段为28字节, 定长
		private static int stringLenth = 76;	//每个字节可以转化为两个字符
		public static bool Analy(DateTime time, List<KeyValuePair<byte, bool>> messgeQueue)
		{
			string messageString = TransferToString(messgeQueue);	//转化为字符串
			//Console.WriteLine(messageString);						//显示, 测试用
			int index = 0;//从第几个字符开始匹配, 相应的, 第几个字节应为:index/2
			if (messgeQueue.Count < 38)
				return false;
			//如果检验到匹配, 将匹配的字节数组(Key)对应的Value置为false, 并返回true;
			if (CheckWeather(messageString, out index))
			{
				index /= 2;
				for (int i = 0; i < byteLength; i++)
					messgeQueue[i + index] = new KeyValuePair<byte, bool>(messgeQueue[i + index].Key, false);
				return true;
			}
			//否则, 返回false;
			return false;
		}

		public static bool CheckWeather(string message, out int index)
		{
			index = 0;
			bool flag = false;
			while (message.Contains("03030020"))//固定报头: 地址&长度
			{
				index = message.IndexOf("03030020");
				string dataString = message.Substring(index, stringLenth - 4);			//数据字串
				byte[] DataByte = SToBa(dataString);						//数据字串对应的数组
				index += (stringLenth - 4);
				string checkSubString = message.Substring(index, 4);	//校验字串
				//byte[] CheckByte = SToBa(checkSubString);				//校验字串数组
				if (CRC16.GetCRC16(DataByte) == checkSubString)
				{
					GetDataString(dataString);
					message.Replace("03030020", "********");
					index -= (stringLenth - 4);
					flag = true;
				}
				else
					return false;
			}
			if (flag)
				return true;
			return false;
		}

		private static string Datastring;
		/// <summary>
		/// 将数据写入数据库
		/// </summary>
		/// <param name="str"></param>
		private static void GetDataString(string str)
		{
			Datastring = str;
			//将数据写入数据库
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
