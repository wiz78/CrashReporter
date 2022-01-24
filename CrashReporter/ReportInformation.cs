namespace info.tellini.CrashReporter
{
	/// <summary>
	/// Information used to initialize the report dialog
	/// </summary>
	public class ReportInformation
	{
		/// <summary>
		/// Name of the user
		/// </summary>
		public string	UserName { get; set; }
		/// <summary>
		/// EMail of the user
		/// </summary>
		public string	EMail { get; set; }
		/// <summary>
		/// Comments about the crash
		/// </summary>
		public string	Comments { get; set; }
	}
}
