using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using info.tellini.CrashReporter.Forms;

namespace info.tellini.CrashReporter
{
	/// <summary>
	/// Intercepts unhandled exceptions and reports them to your bugtracker
	/// </summary>
	public static class CrashReporter
	{
		/// <summary>
		/// Delegate signature for CustomDataDelegate
		/// </summary>
		/// <param name="ex">exception</param>
		/// <returns>extra information to add to shell.txt</returns>
		public delegate	string					CustomDataHandler( Exception ex );
		/// <summary>
		/// Delegate signature for ExceptionFilter
		/// </summary>
		/// <param name="ex">exception</param>
		/// <returns>true if the exception should be reported, false if it's already been handled</returns>
		public delegate	bool					ExceptionFilterHandler( Exception ex );

		/// <summary>
		/// The URL where data will be posted to
		/// </summary>
		public static string					ServerURL { get; set; }
		/// <summary>
		/// Set to true to truncate the reported version to the first two parts (major.minor)
		/// </summary>
		public static bool						UseShortVersion { get; set; }
		/// <summary>
		/// Set to true if it should try to collect recent events with source == the assembly name from the Application log
		/// </summary>
		public static bool						CollectEventLog { get; set; }
		/// <summary>
		/// Proxy to use to report the data to the server, if needed
		/// </summary>
		public static IWebProxy					Proxy { get; set; }

		/// <summary>
		/// May return extra information to include in the report
		/// </summary>
		public static CustomDataHandler			CustomDataDelegate { get; set; }

		/// <summary>
		/// If set, allows to filter exceptions that should be reported.
		/// </summary>
		/// <returns>true if the exception should be reported, false if it's been handled</returns>
		public static ExceptionFilterHandler	ExceptionFilter { get; set; }

		static CrashReporter()
		{
			ServerURL       = ConfigurationManager.AppSettings[ "CrashReporterURL" ];
			UseShortVersion = Convert.ToBoolean( ConfigurationManager.AppSettings[ "CrashReporterUseShortVersion" ] );
			CollectEventLog = Convert.ToBoolean( ConfigurationManager.AppSettings[ "CrashReporterCollectEventLog" ] );
		}

		/// <summary>
		/// Installs the handlers required to intercept unhandled exceptions.
		/// </summary>
		public static void Install()
		{
			Application.ThreadException += ExceptionHandler;

			Application.SetUnhandledExceptionMode( UnhandledExceptionMode.CatchException );
		}

		private static void ExceptionHandler( object sender, ThreadExceptionEventArgs e )
		{
			ReportException( e.Exception );
		}

		/// <summary>
		/// Asks the user if the exception should be reported. If so, collects some data and send it to the server.
		/// </summary>
		/// <remarks>It does nothing at all if ServerURL is empty or ExceptionFilter( ex ) returns false</remarks>
		/// <param name="ex">exception to report</param>
		public static void ReportException( Exception ex )
		{
			if( !string.IsNullOrWhiteSpace( ServerURL ) && (( ExceptionFilter == null ) || ExceptionFilter( ex )))
				try {
					ReportForm		form = new ReportForm();
					List<string>	additionalInfo = new List<string>();

					if( UseShortVersion )
						additionalInfo.Add( "Full version: " + GetAppVersion( false ));

					additionalInfo.Add( GetOpenForms() );
					additionalInfo.Add( GetStackTrace( ex ));

					additionalInfo.RemoveAll( string.IsNullOrWhiteSpace );

					Trace.WriteLine( string.Join( "\n\n", additionalInfo ));

					if( form.ShowDialog() == DialogResult.OK ) {
						WebClient			webClient = new WebClient();
                        NameValueCollection	values = new NameValueCollection 
													 {
														 { "email", "\"" + form.UserName + "\" <" + form.EMail + ">" },
														 { "comment", form.Comments },
														 { "version", GetAppVersion( UseShortVersion ) },
														 { "summary", ex.ToString() },
														 { "crashes", string.Join( "\n\n---\n\n", additionalInfo ) },
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

		private static string GetOpenForms()
		{
			string ret = "";

			try {
				if( Application.OpenForms.Count > 0 ) {

					ret = "Open forms:";

					foreach( Form form in Application.OpenForms ) {
						
						ret += $"\n - {form.GetType().FullName}";

						if( form == Form.ActiveForm )
							ret += " (**active**)";
					}
				}
			}
			catch {
			}

			return ret;
		}

		private static string GetEventLog()
		{
			StringBuilder	ret = new StringBuilder();

			try {
				Assembly	asm = Assembly.GetEntryAssembly();

				if( asm != null ) {
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

				if( asm != null ) {

					ret.AppendLine( "Referenced assemblies:" );

					foreach( AssemblyName an in asm.GetReferencedAssemblies() )
						ret.AppendLine( $"   Name={an.Name}, Version={an.Version}, Culture={an.CultureInfo.Name}, PublicKey token={BitConverter.ToString( an.GetPublicKeyToken() )}" );
				}
			}
			catch {
			}

			return ret.ToString();
		}

		private static string GetSystemData()
		{
			OperatingSystem	os = Environment.OSVersion;
			string			ret = "OS_VERSION = " + os.VersionString;
			string			osName = null, osEdition = null, displayVersion = null;

			using( RegistryKey regKey = Registry.LocalMachine.OpenSubKey( "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion" )) 
				if( regKey != null ) {
					osName         = (string)regKey.GetValue( "ProductName" );
					osEdition      = (string)regKey.GetValue( "EditionID" );
					displayVersion = (string)regKey.GetValue( "DisplayVersion" );
				}

			if( !string.IsNullOrWhiteSpace( displayVersion ))
				ret += $" ({displayVersion})";

			ret += $"\nOS_NAME = {osName ?? OSInfo.Name} ({osEdition ?? OSInfo.Edition})";

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
				string stackTrace = Regex.Replace( exception.StackTrace, @"^\s+", 
												   m => new string( '\u00a0', m.Groups[0].Length ),
												   RegexOptions.Multiline );

				ret += $"**{exception.Message}**\n{stackTrace}\n\n";

				exception = exception.InnerException;

			} while( exception != null );

			return ret;
		}

		private static string GetAppVersion( bool shortVersion )
		{
			string		ret = string.Empty;
			Assembly	ass = Assembly.GetEntryAssembly();

			if( ass != null ) {
				Version	vers = ass.GetName().Version;

				if( shortVersion )
					ret = vers.Major + "." + vers.Minor;
				else
					ret = vers.ToString();
			}
				
			return ret;
		}
	}
}
