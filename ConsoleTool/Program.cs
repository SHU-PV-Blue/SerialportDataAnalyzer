using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

using SHUPV.Database.Core;

namespace SerialportDataAnalyzer
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Title = "串口数据解析器控制台版";
			string sendFilePath;
			string receiveFilePath;

			if(args.Length != 2)
			{
				Console.WriteLine("解析器已启动，请输入完整发送文件名：");
				sendFilePath = Console.ReadLine();

				Console.WriteLine("解析器已启动，请输入完整接收文件名：");
				receiveFilePath = Console.ReadLine();
			}
			else
			{
				sendFilePath = args[0];
				receiveFilePath = args[0];
			}
			

			if ((new FileInfo(sendFilePath)).Exists && (new FileInfo(receiveFilePath)).Exists)
			{
				try
				{
					List<string> errorLog = new List<string>();
					MainAnalyzer analyzer = new MainAnalyzer(sendFilePath,receiveFilePath, errorLog);
					analyzer.Analy();
					StreamWriter errorLogFile = new StreamWriter("ErrorLog.txt", false);
					foreach(var str in errorLog)
						errorLogFile.WriteLine(str);
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.Message);
					Exception exce = ex;
					while(exce != null)
					{
						Console.WriteLine(exce.StackTrace);
						exce = exce.InnerException;
					}
				}
			}
			else
			{
				Console.WriteLine("文件不存在!");
			}

			Console.WriteLine("解析结束，错误报告已经导出至ErrorLog.txt");
		}
	}
}
