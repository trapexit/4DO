using System;
using System.Drawing;
using FourDO.Emulation.FreeDO;

namespace FourDO.UI.Canvases
{
	public delegate void BeforeRenderEventHandler(Size newSize);
	interface ICanvas : IDisposable
	{
		event BeforeRenderEventHandler BeforeRender;

		bool ImageSmoothing { get; set; }
		bool AutoCrop { get; set; }
		bool RenderHighResolution { get; set; }
		bool IsInResizeMode { get; set; }
		ScalingAlgorithm ScalingAlgorithm { get; set; }

		void PushFrame(IntPtr currentFrame);
		Bitmap GetCurrentBitmap();

		void Initialize();
		void Destroy();
	}
}
