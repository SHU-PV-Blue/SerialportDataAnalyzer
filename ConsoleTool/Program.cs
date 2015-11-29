using System;
using System.IO;
using System.Collections.Generic;

namespace SerialportDataAnalyzer
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Title = "串口数据解析器控制台版";
			Console.WriteLine("解析器已启动，请输入完整文件名：");
			string filePath;
			filePath = Console.ReadLine();
			if((new FileInfo(filePath)).Exists)
			{
				try
				{
					List<string> errorLog = new List<string>();
					MainAnalyzer analyzer = new MainAnalyzer(filePath, errorLog);
					analyzer.Analy();
					foreach(var str in errorLog)
						Console.WriteLine(str);
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
			Console.Read();
		}
	}
}
