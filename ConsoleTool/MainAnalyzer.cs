using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace SerialportDataAnalyzer
{
	class MainAnalyzer
	{
		string _filePath;
		List<string> _errorLog;
		public MainAnalyzer(string filePath, List<string> errorLog)
		{
			_filePath = filePath;
			_errorLog = errorLog;
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
				List<KeyValuePair<byte, bool>> messageQueue = new List<KeyValuePair<byte, bool>>();
				//KeyValuePair<byte, bool>表示一个字节，byte指示字节的值，bool表示字节是否可用，若为false则表示改字节已被取走
				DateTime time;
				OleDbConnection oleDbCon = DatabaseConnection.GetConnection();
				oleDbCon.Open();
				for(int lineIndex = 0; lineIndex < lines.Count; ++lineIndex)
				{
					string line = lines[lineIndex];

					//对时间的读取解析
					string dataTimeString = line.Substring(0, line.IndexOf("###"));
					string[] dataTimeStringSplit = dataTimeString.Split(' ');
					string dataString = dataTimeStringSplit[0];
					string timeString = dataTimeStringSplit[1];

					string[] dataStringSplit = dataString.Split('/');
					string[] timeStringSplit = timeString.Split(':');

					int year = Convert.ToInt32(dataStringSplit[0]);
					int month = Convert.ToInt32(dataStringSplit[1]);
					int day = Convert.ToInt32(dataStringSplit[2]);
					int hour = Convert.ToInt32(timeStringSplit[0]);
					int minute = Convert.ToInt32(timeStringSplit[1]);
					int second = Convert.ToInt32(timeStringSplit[2]);
					time = new DateTime(year, month, day, hour, minute, second);

					byte[] data = Transfer.SToBa(line.Substring(line.IndexOf("###") + "###".Length));
					foreach(byte b in data)
					{
						messageQueue.Add(new KeyValuePair<byte, bool>(b, true));
					}
					if (messageQueue.Count >= 2000)
						throw new Exception("消息队列过长!");

					//测试代码,测试对时间和数据的读取
					//Console.WriteLine(time.ToString() + Transfer.BaToS(data));
					//messageQueue.Clear();
					//continue;
					//测试正常

					JDQ32Analyzer.Analy(time, messageQueue);
					JDQ8Analyzer.Analy(time, messageQueue);
#warning 请重新实现VIAnalyzer.Analy(),然后删除这个警告，取消注释
					//VIAnalyzer.Analy(time, messageQueue, oleDbCon);
#warning 请重新实现QXAnalyzer.Analy(),然后删除这个警告，取消注释
					//QXAnalyzer.Analy(time, messageQueue, oleDbCon);
					
					
					while(true)
					{
						//删掉队首已被取走的字节
						while (messageQueue.Count != 0 && !messageQueue[0].Value)
							messageQueue.RemoveAt(0);
						int indexOfFirstNotUse = -1;
						for (int i = 0; i < messageQueue.Count; ++i)
						{
							if (!messageQueue[i].Value)
							{
								indexOfFirstNotUse = i;
								break;
							}
						}

						//如果队列中没有被取走的字节，说明没有出错
						if (indexOfFirstNotUse == -1)
							break;

						//取走队列中的出错的数据
						List<byte> errorData = new List<byte>();
						for (int i = 0; i < indexOfFirstNotUse; ++i)
						{
							errorData.Add(messageQueue[0].Key);
							messageQueue[i] = new KeyValuePair<byte, bool>(messageQueue[i].Key, false);
						}
						_errorLog.Add("Error#" + _filePath + "#" + time + "#" + lineIndex + "#" + Transfer.BaToS(errorData.ToArray()));
					}
				}
				oleDbCon.Close();
			}
			catch(Exception ex)
			{
				throw new Exception(_filePath + "解析失败:" + ex.Message, ex);
			}
			return true;
		}
	}
}

