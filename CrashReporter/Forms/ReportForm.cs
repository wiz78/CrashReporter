﻿using System.Windows.Forms;

namespace info.tellini.CrashReporter.Forms
{
	public partial class ReportForm : Form
	{
		public string UserName => tbName.Text;
		public string EMail => tbEMail.Text;
		public string Comments => tbComments.Text;

		public ReportForm()
		{
			InitializeComponent();
		}
	}
}
