namespace 数据查看器
{
	partial class MainForm
	{
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.dtpDate = new System.Windows.Forms.DateTimePicker();
			this.btnGetData = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// dtpDate
			// 
			this.dtpDate.Location = new System.Drawing.Point(12, 112);
			this.dtpDate.Name = "dtpDate";
			this.dtpDate.Size = new System.Drawing.Size(200, 21);
			this.dtpDate.TabIndex = 0;
			// 
			// btnGetData
			// 
			this.btnGetData.Location = new System.Drawing.Point(51, 181);
			this.btnGetData.Name = "btnGetData";
			this.btnGetData.Size = new System.Drawing.Size(123, 57);
			this.btnGetData.TabIndex = 1;
			this.btnGetData.Text = "导出数据";
			this.btnGetData.UseVisualStyleBackColor = true;
			this.btnGetData.Click += new System.EventHandler(this.btnGetData_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(244, 395);
			this.Controls.Add(this.btnGetData);
			this.Controls.Add(this.dtpDate);
			this.Name = "MainForm";
			this.Text = "数据查看器";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DateTimePicker dtpDate;
		private System.Windows.Forms.Button btnGetData;
	}
}

