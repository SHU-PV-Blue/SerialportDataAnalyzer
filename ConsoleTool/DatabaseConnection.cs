using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.OleDb;

namespace SerialportDataAnalyzer
{
	static class DatabaseConnection
	{
		static public OleDbConnection GetConnection()
		{
			try
			{
				String strConn = @"Provider = Microsoft.Jet.OLEDB.4.0;
							Data Source=.\Database.mdb";
				//这里采用了低版本的access数据库格式以避免不兼容的坑
				return new OleDbConnection(strConn);
			}
			catch(Exception ex)
			{
				throw ex;
			}
		}
	}
}
