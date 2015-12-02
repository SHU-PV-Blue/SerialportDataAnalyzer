using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;

using System.Data;
using System.Data.OleDb;
using SHUPV.Database.Core;

namespace 数据查看器
{
	class Exporter
	{
		DateTime _dt;
		public Exporter(DateTime dt)
		{
			_dt = dt;
			
		}
		public bool Export()
		{
			OleDbConnection dbCon = DatabaseConnection.GetConnection();
			dbCon.Open();
			DatabaseCore dc = new DatabaseCore(dbCon);
			Dictionary<string, string> query = new Dictionary<string,string>();
			query.Add("Year",_dt.Year.ToString());
			query.Add("Month",_dt.Month.ToString());
			query.Add("Day",_dt.Day.ToString());
			System.Data.DataTable ivDt = dc.SelectData("dbo_IVTable", query);
			System.Data.DataTable mDt = dc.SelectData("MeteorologicalData", query);
			dbCon.Close();
			if (ivDt.Rows.Count == 0 && mDt.Rows.Count == 0)
				return false;

            DataView dv = ivDt.DefaultView;
            dv.Sort = "[Hour] ASC, [Minute] ASC, [Second] ASC";
            ivDt = dv.ToTable();
			dv = mDt.DefaultView;
            mDt = dv.ToTable();

			string path = System.Environment.CurrentDirectory + "\\Excel\\";
			Application excel = new Application();
			Workbooks wbks = excel.Workbooks;
			Workbook wb = wbks.Add(path + "mb.xlsx");

			Worksheet wsh = wb.Sheets[1];
			int index = 2;
			foreach(DataRow dr in mDt.Rows)
			{
				//明天继续写
			}






			wb.SaveAs(path + _dt.Year + "年" + _dt.Month + "月" + _dt.Day + "日.xlsx");
			wb.Close();
			return true;
		}
	}
}
