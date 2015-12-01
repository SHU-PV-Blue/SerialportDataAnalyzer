using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;

using System.IO;
using System.Reflection;

namespace 数据查看器
{
	class Exporter
	{
		public Exporter(DateTime dt)
		{
			//查询数据库，之后再写

			Application excel = new Application();//引用Excel对象
			Workbooks wbks = excel.Workbooks;
			Workbook wb = wbks.Add(true);
			excel.Visible = true ;//使Excel可视
			wb.SaveAs("E:\\ad.xlsx");
			wb.Close();
		}
	}
}
