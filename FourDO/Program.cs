using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using FourDO.Utilities;
using FourDO.Utilities.Globals;
using FourDO.Utilities.Logging;

namespace FourDO
{
	static class Program
	{
		private static FourDO.UI.Main mainForm;

		private enum LoggingOption
		{
			AudioDebug,
			AudioTiming,
			CPUTiming
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			Program.InitializeLogging();
			Program.InitializeExceptions();

			// Read command line arguments, and quit if necessary!
			if (!ReadCommandLineArgs(args))
				return;

			Program.SetLanguage();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Trace.WriteLine("4DO Starting up");
			TimingHelper.MaximumResolutionPush();

			mainForm = new FourDO.UI.Main();
			Application.Run(mainForm);

			TimingHelper.MaximumResolutionPop();
			Trace.WriteLine("4DO Shutting down");
			Trace.Flush();

			PrintFakeDosPrompt();
		}

		// Application-wide helper to get the main window handle.
		internal static IntPtr GetMainWindowHwnd()
		{
			if (mainForm == null || !mainForm.IsHandleCreated)
				return IntPtr.Zero;

			return mainForm.Handle;
		}

		private static void InitializeLogging()
		{
			const string TEMPORARY_DIRECTORY_NAME = "Temp";
			const string DEBUG_LOG_NAME = "DebugLog.txt";

			const int MAX_LOG_LENGTH_BYTES = 1 * 1024 * 1024;
			const int MAX_BACKUP_FILES = 10;

			string temporaryDir = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), TEMPORARY_DIRECTORY_NAME);
			string logFileName = Path.Combine(temporaryDir, DEBUG_LOG_NAME);

			// Create the directory if it doesn't already exist.
			if (!Directory.Exists(temporaryDir))
				Directory.CreateDirectory(temporaryDir);

			// Setup a trace listener to our log file!
			var fs = new FileStreamWithBackup(logFileName, MAX_LOG_LENGTH_BYTES, MAX_BACKUP_FILES, FileMode.Append);
			fs.CanSplitData = false;
			var listener = new TextWriterTraceListenerWithTime(fs);

			Trace.Listeners.Add(listener);
		}

		#region Exception Handling

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

		#endregion

		private static void SetLanguage()
		{
			Constants.SetSystemDefaultCulture(System.Threading.Thread.CurrentThread.CurrentUICulture);
			string language = Properties.Settings.Default.Language;
			if (string.IsNullOrWhiteSpace(language))
			{
				// Don't set the language. Use the system default.
			}
			else
			{
				// We will always set the program's culture to whatever is specified.
				// I figured I would do this in case someone added support for their
				// language and selected it in the settings file themselves.
				System.Globalization.CultureInfo cultureInfo = null;
				try
				{
					cultureInfo = System.Globalization.CultureInfo.GetCultureInfo(language);
				}
				catch {}
				
				if (cultureInfo != null)
					System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;
			}
		}

		private static bool ReadCommandLineArgs(string[] args)
		{
			const string USAGE_ERROR = "*** ERROR: ";
			bool errorOccurred = false;

			if (args.Length > 0)
			{
				AttachConsole(-1);
				Console.WriteLine();
			}

			var arguments = new Arguments(args);

			var startLoadFile = arguments["StartLoadFile"];
			if (startLoadFile != null)
				RunOptions.StartLoadFile = startLoadFile;

			var startLoadDrive = arguments["StartLoadDrive"];
			if (startLoadDrive != null)
			{
				if (startLoadDrive.Trim().Length == 1)
				{
					RunOptions.StartLoadDrive = (char)startLoadDrive.Trim().ToCharArray()[0];
				}
				else
				{
					Console.WriteLine(USAGE_ERROR + "The drive letter must be a single character: " + startLoadDrive);
					errorOccurred = true;
				}
			}

			if (arguments["StartFullScreen"] != null)
				RunOptions.StartFullScreen = true;

			if (arguments["DebugStartupPaused"] != null)
				RunOptions.StartupPaused = true;

			var startupFormString = arguments["DebugStartupForm"];
			if (startupFormString != null)
			{
				RunOptions.StartupFormOption startupForm;
				if (Enum.TryParse<RunOptions.StartupFormOption>(startupFormString, out startupForm))
				{
					RunOptions.StartupForm = startupForm;
				}
				else
				{
					Console.WriteLine(USAGE_ERROR + "The supplied startup form wasn't recognized: " + startupFormString);
					errorOccurred = true;
				}
			}

			var loggingOptionsString = arguments["DebugLogging"];
			if (loggingOptionsString != null)
			{
				var logOptionsStrings = loggingOptionsString.Split('|');
				foreach (var logOptionString in logOptionsStrings)
				{
					LoggingOption logOption;
					if (Enum.TryParse<LoggingOption>(logOptionString, out logOption))
					{
						if (logOption == LoggingOption.AudioDebug)
							RunOptions.LogAudioDebug = true;
						if (logOption == LoggingOption.AudioTiming)
							RunOptions.LogAudioTiming = true;
						if (logOption == LoggingOption.CPUTiming)
							RunOptions.LogCPUTiming = true;
					}
					else
					{
						Console.WriteLine(USAGE_ERROR + "The supplied logging option wasn't recognized: " + logOptionString);
						errorOccurred = true;
					}
				}
			}

			if (arguments["ForceGDIRendering"] != null)
				RunOptions.ForceGdiRendering = true;

			if (arguments["printKPrint"] != null)
				RunOptions.PrintKPrint = true;

			bool askedWithQuestionMark = args.Any(x => x == "-?" || x == "/?" || x == "--?");
			if (errorOccurred || askedWithQuestionMark || arguments["help"] != null || arguments["h"] != null)
			{
				Console.WriteLine("======================================================================");
				Console.WriteLine("= 4DO (" + Application.ProductVersion + ") command line options usage                           =");
				Console.WriteLine("=   Basic usage: 4DO.exe [-option value][/option \"value\"][--switch]  =");
				Console.WriteLine("======================================================================");
				Console.WriteLine("");
				Console.WriteLine("  -StartLoadFile [filename] : Loads a game from file.");
				Console.WriteLine("  -StartLoadDrive [letter]  : Loads from CD of the drive letter.");
				Console.WriteLine("  --StartFullScreen         : Start Full Screen.");
				Console.WriteLine("");
				Console.WriteLine("  --PrintKPrint        : Prints KPRINT (3DO debug) output to console.");
				Console.WriteLine("  --ForceGDIRendering  : Forces GDI Rendering rather than DirectX.");
				Console.WriteLine("  --DebugStartupPaused : Start 4do in a paused state.");
				Console.WriteLine("______________________________________________________________________");
				Console.WriteLine("");
				Console.WriteLine("  -DebugLogging [LoggingOption_1|LoggingOption_2]");
				Console.WriteLine("         Enable extra logging to the log files Valid values are:");
				foreach (string val in Enum.GetNames(typeof(LoggingOption)))
					Console.WriteLine("               " + val);
				Console.WriteLine("______________________________________________________________________");
				Console.WriteLine("");
				Console.WriteLine("  -DebugStartupForm [StartupForm]");
				Console.WriteLine("         Starts an extra dialog at startup. Valid values are:");
				foreach (string val in Enum.GetNames(typeof(RunOptions.StartupFormOption)))
				Console.WriteLine("               " + val);
				Console.WriteLine("______________________________________________________________________");
				Console.WriteLine("");
				Console.WriteLine("  -?   /?   --?   -h   /h   --h   -help   /help   --help");
				Console.WriteLine("         Displays this help message.");
				Console.WriteLine("");
				Console.WriteLine("======================================================================");
				Console.WriteLine("");

				PrintFakeDosPrompt();

				return false;
			}

			return true;
		}

		/// <summary>
		/// Heh heh heh
		/// </summary>
		private static void PrintFakeDosPrompt()
		{
			// NOTE: I'm cheating and "redrawing" the command-line prompt.
			Console.Write(System.IO.Directory.GetCurrentDirectory() + ">");
		}

		[System.Runtime.InteropServices.DllImport("kernel32.dll")]
		private static extern bool AllocConsole();

		[System.Runtime.InteropServices.DllImport("kernel32.dll")]
		private static extern bool AttachConsole(int pid);
	}
}
