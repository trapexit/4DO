using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FourDO.UI.Canvases
{
	public delegate void BeforeRenderEventHandler(Size newSize);
	interface ICanvas : IDisposable
	{
		event BeforeRenderEventHandler BeforeRender;

		bool ImageSmoothing { get; set; }
		bool RenderHighResolution { get; set; }
		bool IsInResizeMode { get; set; }

		void PushFrame(IntPtr currentFrame);
		Bitmap GetCurrentBitmap();

		void Initialize();
		void Destroy();
	}
}
