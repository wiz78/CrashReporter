using System;
using System.Windows.Forms;
using info.tellini.CrashReporter;

namespace CrashTest
{
	static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			CrashReporter.Install();

			Application.SetHighDpiMode( HighDpiMode.SystemAware );
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );
			Application.Run( new Form1() );
		}
	}
}
