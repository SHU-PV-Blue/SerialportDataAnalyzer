using System;
using System.IO;

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
					MainAnalyzer analyzer = new MainAnalyzer(filePath);
					analyzer.Analy();
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.Message);
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
