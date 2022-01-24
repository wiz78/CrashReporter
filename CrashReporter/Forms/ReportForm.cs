using System.Windows.Forms;

namespace info.tellini.CrashReporter.Forms
{
	internal partial class ReportForm : Form
	{
		public ReportInformation	Info => new ReportInformation
											{
												UserName = tbName.Text,
												EMail = tbEMail.Text,
												Comments = tbComments.Text,
											};

		public ReportForm( ReportInformation reportInformation )
		{
			InitializeComponent();

			if( reportInformation != null ) {

				tbName.Text = reportInformation.UserName;
				tbEMail.Text = reportInformation.EMail;
				tbComments.Text = reportInformation.Comments;
			}
		}
	}
}
