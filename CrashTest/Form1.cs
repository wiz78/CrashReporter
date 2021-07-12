using System;
using System.Windows.Forms;

namespace CrashTest
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			throw new Exception( "Crash test" );
		}
	}
}
