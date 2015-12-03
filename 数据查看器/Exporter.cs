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

			int row = 2;
			foreach(DataRow dr in mDt.Rows)
			{
				Worksheet wsh = wb.Sheets[3];
				int col = 1;
				wsh.Cells[row, col++] = dr["Hour"] + ":" + dr["Minute"] + ":" + dr["Second"];
				wsh.Cells[row, col++] = dr["WindSpeed(m/s)"];
				wsh.Cells[row, col++] = dr["AirTemperayure"];
				wsh.Cells[row, col++] = dr["Rasiation(W/m2)"];
				wsh.Cells[row, col++] = dr["WindDirection"];
				wsh.Cells[row, col++] = dr["Humidity(%RH)"];

				DataRow[] result = ivDt.Select("[Hour] = " + dr["Hour"] + " and " + "[Minute] = " + dr["Minute"] + " and " + "[Second] = " + dr["Second"]);
				wsh.Cells[row, col++] = result[0]["Component1Temperature"];

				wsh.Cells[row, col++] = dr["Component2Temperature"];
				wsh.Cells[row, col++] = dr["Component3Temperature"];
				wsh.Cells[row, col++] = dr["Component4Temperature"];
				wsh.Cells[row, col++] = dr["Component5Temperature"];
				wsh.Cells[row, col++] = dr["Component6Temperature"];

				row++;
			}

			int sheet1Row = 4;
			int sheet2Row = 3;
			foreach (DataRow dr in ivDt.Rows)
			{
				int sheet;
				int col;
				GetLocation((int)dr["ComponentId"], (int)dr["Azimuth"], (int)dr["Obliquity"], out sheet, out col);
				Worksheet wsh = wb.Sheets[sheet];
				if(sheet == 1)
				{
					wsh.Cells[sheet1Row, 1] = dr["Hour"] + ":" + dr["Minute"] + ":" + dr["Second"];
					wsh.Cells[sheet1Row, col++] = dr["OpenCircuitVoltage"];
					wsh.Cells[sheet1Row, col++] = dr["ShortCircuitCurrent"];
					wsh.Cells[sheet1Row, col++] = dr["MaxPowerVoltage"];
					wsh.Cells[sheet1Row, col++] = dr["MaxPowerCurrent"];
					wsh.Cells[sheet1Row, col++] = dr["MaxPower"];
					++sheet1Row;
				}
				if (sheet == 2)
				{
					wsh.Cells[sheet2Row, 1] = dr["Hour"] + ":" + dr["Minute"] + ":" + dr["Second"];
					wsh.Cells[sheet2Row, col++] = dr["OpenCircuitVoltage"];
					wsh.Cells[sheet2Row, col++] = dr["ShortCircuitCurrent"];
					wsh.Cells[sheet2Row, col++] = dr["MaxPowerVoltage"];
					wsh.Cells[sheet2Row, col++] = dr["MaxPowerCurrent"];
					wsh.Cells[sheet2Row, col++] = dr["MaxPower"];
					++sheet2Row;
				}
			}

			wb.SaveAs(path + _dt.Year + "年" + _dt.Month + "月" + _dt.Day + "日.xlsx");
			wb.Close();
			wbks.Close();
			return true;
		}

		private void GetLocation(int componentId,int azimuth, int obliquity, out int sheet, out int startCol)
		{
			switch(componentId)
			{
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				{
					sheet = 2;
					break;
				}
				case 6:
				{
					sheet = 1;
					break;
				}
				default:
				{
					throw new Exception("componentId 参数不合法:" + componentId);
				}
			}
			if(sheet == 1)//是6号组件
			{
				int index = 2;
				switch(azimuth)
				{
					case -10:
						index += 20 * 0;
						break;
					case -5:
						index += 20 * 1;
						break;
					case 0:
						index += 20 * 2;
						break;
					case 5:
						index += 20 * 3;
						break;
					default:
						throw new Exception("azimuth 参数不合法:" + azimuth);
				}
				if (azimuth == -10 || azimuth == 0)
				{
					switch (obliquity)
					{
						case 22:
							index += 0 * 5;
							break;
						case 27:
							index += 1 * 5;
							break;
						case 32:
							index += 2 * 5;
							break;
						case 37:
							index += 3 * 5;
							break;
						default:
							throw new Exception("obliquity 参数不合法:" + obliquity);
					}
				}
				else
				{
					switch (obliquity)
					{
						case 37:
							index += 0 * 5;
							break;
						case 32:
							index += 1 * 5;
							break;
						case 27:
							index += 2 * 5;
							break;
						case 22:
							index += 3 * 5;
							break;
						default:
							throw new Exception("obliquity 参数不合法:" + obliquity);
					}
				}
				startCol = index;
			}
			else//是1-5号组件
			{
				startCol = 2 + (componentId - 1) * 5;
			}
		}
	}
}
