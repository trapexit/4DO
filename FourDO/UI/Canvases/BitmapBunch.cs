using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace FourDO.UI.Canvases
{
	/// <summary>
	/// This class holds bitmaps from the core emulation.
	/// It provides the "triple-buffered" behavior.
	/// A "prepare bitmap" is a bitmap that needs to have raw data written to it.
	/// A "render bitmap" is a bitmap that is ready for rendering.
	/// </summary>
	internal class BitmapBunch
	{
		private const int BITMAP_COUNT = 3;

		public int BitmapWidth { get; private set; }
		public int BitmapHeight { get; private set; }
		public PixelFormat BitmapPixelFormat { get; private set; }

		public BitmapBunch(int bitmapWidth, int bitmapHeight, PixelFormat bitmapPixelFormat)
		{
			if (bitmapWidth <= 0)
				throw new ArgumentException("Bitmap width must be greater than zero");

			if (bitmapHeight <= 0)
				throw new ArgumentException("Bitmap height must be greater than zero");

			this.BitmapWidth = bitmapWidth;
			this.BitmapHeight = bitmapHeight;
			this.BitmapPixelFormat = bitmapPixelFormat;

			this.blackBitmap = new Bitmap(bitmapWidth, bitmapHeight, bitmapPixelFormat);

			bitmaps = new List<Bitmap>();
			for (int x = 0; x <= BITMAP_COUNT; x++)
			{
				var bitmap = new Bitmap(bitmapWidth, bitmapHeight, bitmapPixelFormat);
				bitmaps.Add(bitmap);
			}

			this.lastPreparedBitmap = null;
			this.currentRenderedBitmap = null;
		}

		public Bitmap GetBlackBitmap()
		{
			return blackBitmap;
		}

		public Bitmap GetNextPrepareBitmap()
		{
			lock (bitmapSemaphore)
			{
				Bitmap returnedBitmap = bitmaps.FirstOrDefault(x => x != this.currentRenderedBitmap && x != this.lastPreparedBitmap);
				return returnedBitmap;
			}
		}

		public void SetLastPreparedBitmap(Bitmap bitmap)
		{
			lock (bitmapSemaphore)
			{
				// Make sure they sent us a bitmap we actually own.
				if (!bitmaps.Any(x => x == bitmap))
					return;

				this.lastPreparedBitmap = bitmap;
			}
		}

		public Bitmap GetNextRenderBitmap()
		{
			lock (bitmapSemaphore)
			{
				Bitmap returnedBitmap = this.lastPreparedBitmap;
				this.currentRenderedBitmap = returnedBitmap;
				return returnedBitmap;
			}
		}

		public void ClearImages()
		{
			lock (bitmapSemaphore)
			{
				this.lastPreparedBitmap = null;
			}
		}

		private object bitmapSemaphore = new object();

		private List<Bitmap> bitmaps;
		private Bitmap blackBitmap;

		private Bitmap currentRenderedBitmap;
		private Bitmap lastPreparedBitmap;

	}
}
