using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace SerialportDataAnalyzer
{
	class MainAnalyzer
	{
		string _sendFilePath;
		string _receiveFilePath;
		List<string> _errorLog;
		public MainAnalyzer(string sendFilePath, string receiveFilePath, List<string> errorLog)
		{
			_sendFilePath = sendFilePath;
			_receiveFilePath = receiveFilePath;
			_errorLog = errorLog;
		}
		public bool Analy()
		{

			List<KeyValuePair<DateTime, string>> receiveLines = LoadLines(_receiveFilePath);
			List<KeyValuePair<DateTime, string>> sendLines = LoadLines(_sendFilePath);
			try
			{
				List<KeyValuePair<byte, bool>> messageQueue = new List<KeyValuePair<byte, bool>>();
				//KeyValuePair<byte, bool>表示一个字节，byte指示字节的值，bool表示字节是否可用，若为false则表示改字节已被取走

				ComponentAnalyzer componentAnalyzer = new ComponentAnalyzer(sendLines);

				OleDbConnection oleDbCon = DatabaseConnection.GetConnection();
				oleDbCon.Open();
				for(int lineIndex = 0; lineIndex < receiveLines.Count; ++lineIndex)
				{
					string line = receiveLines[lineIndex].Value;
					DateTime time = receiveLines[lineIndex].Key;
					byte[] data = Transfer.SToBa(line);

					foreach(byte b in data)
						messageQueue.Add(new KeyValuePair<byte, bool>(b, true));

					if (messageQueue.Count >= 2000)
						throw new Exception("消息队列过长!");

					//测试代码,测试对解析到哪儿
					Console.WriteLine("解析到" + time.ToString() + Transfer.BaToS(data));


					int componentId, azimuth, obliquity;
					componentAnalyzer.Analy(time, out componentId, out azimuth, out obliquity);
#warning 修改VI曲线仪解析，接受角度、组件号参数
					//VIAnalyzer.Analy(time, messageQueue, oleDbCon, componentId, azimuth, obliquity);
					QXAnalyzer.Analy(time, messageQueue, oleDbCon);
					
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
						_errorLog.Add("Error#" + _receiveFilePath + "#" + time + "#" + lineIndex + "#" + Transfer.BaToS(errorData.ToArray()));
					}
				}
				oleDbCon.Close();
			}
			catch(Exception ex)
			{
				throw new Exception(_receiveFilePath + "解析失败:" + ex.Message, ex);
			}
			return true;
		}

		private List<KeyValuePair<DateTime, string>> LoadLines(string filePath)
		{
			
			List<KeyValuePair<DateTime, string>> lines = new List<KeyValuePair<DateTime, string>>();
			try
			{
				StreamReader fileIn = new StreamReader(filePath);
				while(!fileIn.EndOfStream)
				{
					string newLine = fileIn.ReadLine();
					if(String.IsNullOrEmpty(newLine) && fileIn.EndOfStream)
						break;

					string dataTimeString = newLine.Substring(0, newLine.IndexOf("###"));
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

					lines.Add(new KeyValuePair<DateTime, string>(new DateTime(year, month, day, hour, minute, second),
									(newLine.Substring(newLine.IndexOf("###") + "###".Length))));
				}
				fileIn.Close();
				if (lines.Count == 0)
					throw new Exception("只读到0行数据:" + filePath);
			}
			catch(Exception ex)
			{
				throw new Exception(filePath + "读取失败:" + ex.Message, ex);
			}
			return lines;
		}
	}
}

