using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FourDO.Utilities
{
	public class FormDrawLocker : IDisposable
	{
		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);
		private const int WM_SETREDRAW = 11; 

		public Form Form
		{
			get { return _form; }
		}

		private Form _form;

		public FormDrawLocker(Form form)
		{
			_form = form;
			SendMessage(_form.Handle, WM_SETREDRAW, false, 0);
		}

		public void Dispose()
		{
			_form.ResumeLayout();
			SendMessage(_form.Handle, WM_SETREDRAW, true, 0);
			_form.Refresh();
		}
	}
}
