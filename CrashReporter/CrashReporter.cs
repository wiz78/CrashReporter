﻿using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using info.tellini.CrashReporter.Forms;

namespace info.tellini.CrashReporter
{
	public static class CrashReporter
	{
		public delegate	string			CustomDataHandler( Exception ex );

		public static string			ServerURL { get; set; }
		public static bool				UseShortVersion { get; set; }
		public static bool				CollectEventLog { get; set; }
		public static IWebProxy			Proxy { get; set; }
		public static CustomDataHandler	CustomDataDelegate { get; set; }

		static CrashReporter()
		{
			ServerURL       = ConfigurationManager.AppSettings[ "CrashReporterURL" ];
			UseShortVersion = Convert.ToBoolean( ConfigurationManager.AppSettings[ "CrashReporterUseShortVersion" ] );
			CollectEventLog = Convert.ToBoolean( ConfigurationManager.AppSettings[ "CrashReporterCollectEventLog" ] );
		}

		public static void Install()
		{
			Application.ThreadException += ExceptionHandler;

			Application.SetUnhandledExceptionMode( UnhandledExceptionMode.CatchException );
		}

		private static void ExceptionHandler( object sender, ThreadExceptionEventArgs e )
		{
			ReportException( e.Exception );
		}

		public static void ReportException( Exception ex )
		{
			if( !string.IsNullOrWhiteSpace( ServerURL ))
				try {
					ReportForm	form = new ReportForm();
					string		stackTrace = GetStackTrace( ex );

					Trace.WriteLine( stackTrace );

					if( form.ShowDialog() == DialogResult.OK ) {
						WebClient			webClient = new WebClient();
                        NameValueCollection	values = new NameValueCollection 
													 {
														 { "email", "\"" + form.UserName + "\" <" + form.EMail + ">" },
														 { "comment", form.Comments },
														 { "version", GetAppVersion( UseShortVersion ) },
														 { "summary", ex.ToString() },
														 { "crashes", stackTrace },
														 { "system", GetSystemData() },
														 { "shell", GetShellData( ex ) },
														 { "preferences", GetAppConfig() },
													 };

						if( CollectEventLog )
							values[ "eventlog" ] = GetEventLog();

						if( Proxy != null )
							webClient.Proxy = Proxy;

						webClient.Headers[ HttpRequestHeader.UserAgent ]  = Application.ProductName + "/" + Application.ProductVersion;

						webClient.UploadValuesAsync( new Uri( ServerURL ), values );
					}
				}
				catch {
				}
		}

		private static string GetEventLog()
		{
			StringBuilder	ret = new StringBuilder();

			if( UseShortVersion )
				ret.Append( "Full version: " ).AppendLine( GetAppVersion( false )).AppendLine();

			try {
				Assembly	asm = Assembly.GetEntryAssembly();
				string		source = asm.GetName( false ).Name;
				EventLog[]	events = EventLog.GetEventLogs();
				DateTime	start = DateTime.Now.AddMinutes( -30 );

				foreach( EventLog log in events )
					foreach( EventLogEntry entry in log.Entries )
						if(( entry.Source == source ) && ( entry.TimeGenerated >= start )) {
							
							ret.Append( "[" ).Append( entry.TimeGenerated.ToString( "o" )).Append( "] " );
							ret.Append( entry.EntryType ).Append( " - " );
							ret.Append( entry.Message );
						}
			}
			catch {
			}

			return ret.ToString();
		}

		private static string GetAppConfig()
		{
			string	ret = string.Empty;

			try {
				Configuration	cfg = ConfigurationManager.OpenExeConfiguration( ConfigurationUserLevel.None );

				if( cfg.HasFile ) {
					StringBuilder	str = new StringBuilder();

					ret = File.ReadAllText( cfg.FilePath );

					foreach( ConfigurationSection section in cfg.Sections ) {
						string xml = section.SectionInformation.GetRawXml();

						if( !string.IsNullOrWhiteSpace( xml ) && !ret.Contains( xml )) {

							if( str.Length == 0 )
								str.AppendLine( "\n\nConfigurationSections --------------------------------------" );

							str.AppendLine( xml );
						}
					}

					ret += str.ToString();
				}
			}
			catch {
			}

			return ret;
		}

		private static string GetShellData( Exception ex )
		{
			StringBuilder	ret = new StringBuilder();

			try {
				if( CustomDataDelegate != null )
					ret.AppendLine( CustomDataDelegate( ex )).AppendLine();
			}
			catch {
			}

			try {
				Assembly	asm = Assembly.GetEntryAssembly();

				ret.AppendLine( "Referenced assemblies:" );

				foreach( AssemblyName an in asm.GetReferencedAssemblies() )
					ret.AppendFormat( "   Name={0}, Version={1}, Culture={2}, PublicKey token={3}", 
									  an.Name, an.Version, an.CultureInfo.Name, BitConverter.ToString( an.GetPublicKeyToken() )).AppendLine();
			}
			catch {
			}

			return ret.ToString();
		}

		private static string GetSystemData()
		{
			OperatingSystem	os = Environment.OSVersion;
			string			ret = "OS_VERSION = " + os.VersionString;

			ret += $"\nOS_NAME = {OSInfo.Name} ({OSInfo.Edition})";

			using( RegistryKey regKey = Registry.LocalMachine.OpenSubKey( "HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0" ))
				if( regKey != null )
					ret += "\nCPU_TYPE = " + ( regKey.GetValue( "ProcessorNameString" ) ?? "" );

			ret += $"\nPLATFORM = {OSInfo.Bits} bits";

			return ret;
		}

		private static string GetStackTrace( Exception exception )
		{
			string ret = string.Empty;
			
			do {

				ret += exception.Message + "\n" + exception.StackTrace + "\n\n";

				exception = exception.InnerException;

			} while( exception != null );

			return ret;
		}

		private static string GetAppVersion( bool shortVersion )
		{
			Assembly	ass = Assembly.GetEntryAssembly();
			Version		vers = ass.GetName().Version;
			string		ret;

			if( shortVersion )
				ret = vers.Major + "." + vers.Minor;
			else
				ret = vers.ToString();
				
			return ret;
		}
	}
}
