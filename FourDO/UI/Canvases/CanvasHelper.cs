using System;
using System.Drawing;
using System.Drawing.Imaging;
using FourDO.Emulation.FreeDO;

namespace FourDO.UI.Canvases
{
	internal static class CanvasHelper
	{
		static readonly byte[] FIXED_CLUTR = new byte[32];
		static readonly byte[] FIXED_CLUTG = new byte[32];
		static readonly byte[] FIXED_CLUTB = new byte[32];

		static CanvasHelper()
		{
			for(int j = 0; j < 32; j++)
			{
				FIXED_CLUTR[j] = (byte)(((j & 0x1f) << 3) | ((j >> 2) & 7));
				FIXED_CLUTG[j] = FIXED_CLUTR[j];
				FIXED_CLUTB[j] = FIXED_CLUTR[j];
			}
		}

		public unsafe static void CopyBitmap(IntPtr currentFrame, BitmapDefinition bitmapDefinition, int copyWidth, int copyHeight, bool addBlackBorder, bool copyPointlessAlphaByte, bool allowCrop, ScalingAlgorithm scalingAlgorithm)
		{
			Bitmap bitmapToPrepare = bitmapDefinition.Bitmap;
			BitmapData bitmapData = bitmapToPrepare.LockBits(new Rectangle(0, 0, bitmapToPrepare.Width, bitmapToPrepare.Height), ImageLockMode.WriteOnly, bitmapToPrepare.PixelFormat);

			int newWidth = 0;
			int newHeight = 0;
			FreeDOCore.GetFrameBitmap(
					currentFrame
					, bitmapData.Scan0
					, bitmapToPrepare.Width
					, bitmapDefinition.Crop
					, copyWidth
					, copyHeight
					, addBlackBorder
					, copyPointlessAlphaByte
					, allowCrop
					, scalingAlgorithm
					, out newWidth
					, out newHeight);

			bitmapToPrepare.UnlockBits(bitmapData);
		}

        public static BitmapBunch CreateNewBitmapBunch(bool highResolution, ScalingAlgorithm algorithm, int minWidth, int minHeight, BitmapBunch oldBitmapBunch, PixelFormat format)
        {
            int sizeMultiplier = 1;

            if (highResolution)
                sizeMultiplier *= 2;

            if (algorithm == ScalingAlgorithm.Hq2X)
                sizeMultiplier *= 2;
            else if (algorithm == ScalingAlgorithm.Hq3X)
                sizeMultiplier *= 3;
            else if (algorithm == ScalingAlgorithm.Hq4X)
                sizeMultiplier *= 4;

            int newWidth = minWidth * sizeMultiplier;
            int newHeight = minHeight * sizeMultiplier;

            if (oldBitmapBunch != null
                && newWidth == oldBitmapBunch.BitmapWidth 
                && newHeight == oldBitmapBunch.BitmapHeight 
                && highResolution == oldBitmapBunch.HighResolution
                && algorithm == oldBitmapBunch.ScalingAlgorithm)
                return null;

            // Create new bitmap bunch if necessary.
            var newBunch = new BitmapBunch(minWidth * sizeMultiplier, minHeight * sizeMultiplier, format);
            newBunch.HighResolution = highResolution;
            newBunch.ScalingAlgorithm = algorithm;
            return newBunch;
        }
	}
}
