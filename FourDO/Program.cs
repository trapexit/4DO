using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FourDO
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			string startForm = null;
			if (args.Length >= 2 && args[0].ToLower() == "-debugstartform")
				startForm = args[1];

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			var mainForm = new FourDO.UI.Main();
			mainForm.StartForm = startForm;

			Application.Run(mainForm);
		}
	}
}
