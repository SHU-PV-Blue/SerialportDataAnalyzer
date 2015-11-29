using System;
using System.IO;
using System.Collections.Generic;

namespace SerialportDataAnalyzer
{
	class MainAnalyzer
	{
		string _filePath;
		public MainAnalyzer(string filePath)
		{
			_filePath = filePath;
		}
		public bool Analy()
		{
			StreamReader fileIn;
			List<string> lines = new List<string>();
			try
			{
				fileIn = new StreamReader(_filePath);
				while(!fileIn.EndOfStream)
				{
					string newLine = fileIn.ReadLine();
					if(String.IsNullOrEmpty(newLine) && fileIn.EndOfStream)
						break;
					//FIXME: 这里最好用正则表达式检查一下newLine
					lines.Add(newLine);
				}
				fileIn.Close();
				if (lines.Count == 0)
					throw new Exception("只读到0行数据");
			}
			catch(Exception ex)
			{
				throw new Exception(_filePath + "读取失败:" + ex.Message);
			}

			try
			{
				List<KeyValuePair<byte, bool>> messgeQueue = new List<KeyValuePair<byte, bool>>();
				//KeyValuePair<byte, bool>表示一个字节，byte指示字节的值，bool表示字节是否可用，若为false则表示改字节已被取走

				DateTime time;
				foreach(var line in lines)
				{
					int year = Convert.ToInt32(line.Substring(0, 4));
					int month = Convert.ToInt32(line.Substring(5, 2));
					int day = Convert.ToInt32(line.Substring(8, 2));
					int hour = Convert.ToInt32(line.Substring(11, 2));
					int minute = Convert.ToInt32(line.Substring(14, 2));
					int second = Convert.ToInt32(line.Substring(17, 2));
					time = new DateTime(year, month, day, hour, minute, second);
					//FIXME:用正则表达式会不会好一点？
					byte[] data = Transfer.SToBa(line.Substring(line.IndexOf("###") + "###".Length));
					foreach(byte b in data)
					{
						messgeQueue.Add(new KeyValuePair<byte, bool>(b, true));
					}
					if (messgeQueue.Count >= 2000)
						throw new Exception("消息队列过长!");

					JDQ32Analyzer.Analy(time, messgeQueue);
					JDQ8Analyzer.Analy(time, messgeQueue);
					VIAnalyzer.Analy(time, messgeQueue);
					QXAnalyzer.Analy(time, messgeQueue);

					while(messgeQueue.Count != 0 && !messgeQueue[0].Value)
						messgeQueue.RemoveAt(0);
					//if()
					//TODO:如果消息队列中间有false的键值记录错误档案
					//明天继续从这儿写
				}
			}
			catch(Exception ex)
			{
				throw new Exception(_filePath + "解析失败:" + ex.Message);
			}
			return true;
		}
	}
}
