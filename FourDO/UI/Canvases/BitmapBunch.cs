using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using FourDO.Emulation.FreeDO;

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

		// NOTE: These are in here because the bitmap bunch acts as a container of "settings" used
		// to extract the bitmap when frames are done. We need these settings changes to occur
		// atomically with new sets of bitmaps, so these are just copied along for the ride.
		public ScalingAlgorithm ScalingAlgorithm  { get; set; }
		public bool HighResolution { get; set; }

		public BitmapBunch(int bitmapWidth, int bitmapHeight, PixelFormat bitmapPixelFormat)
		{
			if (bitmapWidth <= 0)
				throw new ArgumentException("Bitmap width must be greater than zero");

			if (bitmapHeight <= 0)
				throw new ArgumentException("Bitmap height must be greater than zero");

			this.BitmapWidth = bitmapWidth;
			this.BitmapHeight = bitmapHeight;
			this.BitmapPixelFormat = bitmapPixelFormat;

			this.blackBitmap = new BitmapDefinition(bitmapWidth, bitmapHeight, bitmapPixelFormat);

			bitmaps = new List<BitmapDefinition>();
			for (int x = 0; x <= BITMAP_COUNT; x++)
			{
				var bitmapDefinition = new BitmapDefinition(bitmapWidth, bitmapHeight, bitmapPixelFormat);
				bitmaps.Add(bitmapDefinition);
			}

			this.lastPreparedBitmap = null;
			this.currentRenderedBitmap = null;
		}

		public BitmapDefinition GetBlackBitmap()
		{
			return blackBitmap;
		}

		public BitmapDefinition GetNextPrepareBitmap()
		{
			lock (bitmapSemaphore)
			{
				BitmapDefinition returnedBitmap = bitmaps.FirstOrDefault(x => x != this.currentRenderedBitmap && x != this.lastPreparedBitmap);
				return returnedBitmap;
			}
		}

		public void SetLastPreparedBitmap(BitmapDefinition bitmap)
		{
			lock (bitmapSemaphore)
			{
				// Make sure they sent us a bitmap we actually own.
				if (!bitmaps.Any(x => x == bitmap))
					return;

				this.lastPreparedBitmap = bitmap;
			}
		}

		public BitmapDefinition GetNextRenderBitmap()
		{
			lock (bitmapSemaphore)
			{
				BitmapDefinition returnedBitmap = this.lastPreparedBitmap;
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

		private readonly object bitmapSemaphore = new object();

		private List<BitmapDefinition> bitmaps;
		private BitmapDefinition blackBitmap;

		private BitmapDefinition currentRenderedBitmap;
		private BitmapDefinition lastPreparedBitmap;

	}
}
