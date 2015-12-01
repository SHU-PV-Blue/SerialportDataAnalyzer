using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 数据查看器
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			dtpDate.Value = DateTime.Now;
		}

		private void btnGetData_Click(object sender, EventArgs e)
		{
			Exporter ex = new Exporter(dtpDate.Value);
		}
	}
}
