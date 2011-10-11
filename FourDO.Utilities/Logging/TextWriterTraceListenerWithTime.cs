// http://www.codeproject.com/KB/dotnet/customnettracelisteners.aspx

using System;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace FourDO.Utilities.Logging
{
	public class TextWriterTraceListenerWithTime : TextWriterTraceListener
	{
		public TextWriterTraceListenerWithTime()
			: base()
		{
		}

		public TextWriterTraceListenerWithTime(Stream stream)
			: base(stream)
		{
		}

		public TextWriterTraceListenerWithTime(string path)
			: base(path)
		{
		}

		public TextWriterTraceListenerWithTime(TextWriter writer)
			: base(writer)
		{
		}

		public TextWriterTraceListenerWithTime(Stream stream, string name)
			: base(stream, name)
		{
		}

		public TextWriterTraceListenerWithTime(string path, string name)
			: base(path, name)
		{
		}

		public TextWriterTraceListenerWithTime(TextWriter writer, string name)
			: base(writer, name)
		{
		}

		public override void WriteLine(string message)
		{
			int indentLevel = base.IndentLevel;
			base.IndentLevel = 0;

			base.Write(DateTime.Now.ToString());
			if (indentLevel == 0)
			{
				base.Write(" ");
			}
			else
			{
				for (int x = 0; x < indentLevel; x++)
				{
					base.Write("    ");
				}
			}

			base.WriteLine(message);

			base.IndentLevel = indentLevel;
		}
	}
}
