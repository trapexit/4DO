using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using FourDO.Utilities;
using FourDO.Utilities.Logging;

namespace FourDO
{
	static class Program
	{
		private static FourDO.UI.Main mainForm;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			Program.InitializeLogging();
			Program.InitializeExceptions();

			string startForm = null;
			if (args.Length >= 2 && args[0].ToLower() == "-debugstartform")
				startForm = args[1];
			
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Trace.WriteLine("4DO Starting up");
			TimingHelper.MaximumResolutionPush();

			mainForm = new FourDO.UI.Main();
			mainForm.StartForm = startForm;
			Application.Run(mainForm);

			TimingHelper.MaximumResolutionPop();
			Trace.WriteLine("4DO Shutting down");
		}

		private static void InitializeLogging()
		{
			const string TEMPORARY_DIRECTORY_NAME = "Temp";
			const string DEBUG_LOG_NAME = "DebugLog.txt";

			const int MAX_LOG_LENGTH_BYTES = 100 * 1024;
			const int MAX_BACKUP_FILES = 10;

			string temporaryDir = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), TEMPORARY_DIRECTORY_NAME);
			string logFileName = Path.Combine(temporaryDir, DEBUG_LOG_NAME);

			// Create the directory if it doesn't already exist.
			if (!Directory.Exists(temporaryDir))
				Directory.CreateDirectory(temporaryDir);

			// Setup a trace listener to our log file!
			FileStreamWithBackup fs = new FileStreamWithBackup(logFileName, MAX_LOG_LENGTH_BYTES, MAX_BACKUP_FILES, FileMode.Append);
			fs.CanSplitData = false;
			TextWriterTraceListenerWithTime listener = new TextWriterTraceListenerWithTime(fs);

			Trace.Listeners.Add(listener);
		}

		private static void InitializeExceptions()
		{
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Trace.WriteLine("Unhandled exception: " + e.ExceptionObject.ToString());
			Trace.Flush();
		}

		private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			Trace.WriteLine("Thread exception: " + e.Exception.ToString());
			Trace.Flush();
		}

		internal static IntPtr GetMainWindowHwnd()
		{
			if (mainForm == null || !mainForm.IsHandleCreated)
				return IntPtr.Zero;

			return mainForm.Handle;
		}
	}
}
