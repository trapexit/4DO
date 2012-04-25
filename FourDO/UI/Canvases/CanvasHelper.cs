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

		public unsafe static void CopyBitmap(IntPtr currentFrame, BitmapDefinition bitmapDefinition, int copyWidth, int copyHeight, bool addBlackBorder, bool copyPointlessAlphaByte, bool allowCrop)
		{
			Bitmap bitmapToPrepare = bitmapDefinition.Bitmap;

			float maxCropPercent = allowCrop ? .25f : 0;
			int maxCropTall = (int)(copyHeight * maxCropPercent);
			int maxCropWide = (int)(copyWidth * maxCropPercent);

			BitmapCrop newCrop = new BitmapCrop();
			newCrop.Top = maxCropTall;
			newCrop.Left = maxCropWide;
			newCrop.Right = maxCropWide;
			newCrop.Bottom = maxCropTall;

			BitmapData bitmapData = bitmapToPrepare.LockBits(new Rectangle(0, 0, bitmapToPrepare.Width, bitmapToPrepare.Height), ImageLockMode.WriteOnly, bitmapToPrepare.PixelFormat);
			int bitmapStride = bitmapData.Stride;
			int pointlessAlphaByte = copyPointlessAlphaByte ? 1 : 0;

			unsafe
			{
				byte* destPtr = (byte*)bitmapData.Scan0.ToPointer();
				VDLFrame* framePtr = (VDLFrame*)currentFrame.ToPointer();
				for (int line = 0; line < copyHeight; line++)
				{
					VDLLine* linePtr = (VDLLine*)&(framePtr->lines[sizeof(VDLLine) * line]);
					short* srcPtr = (short*)linePtr;
					bool allowFixedClut = (linePtr->xOUTCONTROLL & 0x2000000) > 0;
					for (int pix = 0; pix < copyWidth; pix++)
					{
						byte bPart = 0;
						byte gPart = 0;
						byte rPart = 0;
						if (*srcPtr == 0)
						{
							bPart = (byte)(linePtr->xBACKGROUND & 0x1F);
							gPart = (byte)((linePtr->xBACKGROUND >> 5) & 0x1F);
							rPart = (byte)((linePtr->xBACKGROUND >> 10) & 0x1F);
						}
						else if (allowFixedClut && (*srcPtr & 0x8000) > 0)
						{
							bPart = FIXED_CLUTB[(*srcPtr) & 0x1F];
							gPart = FIXED_CLUTG[((*srcPtr) >> 5) & 0x1F];
							rPart = FIXED_CLUTR[(*srcPtr) >> 10 & 0x1F];
						}
						else
						{
							bPart = (byte)(linePtr->xCLUTB[(*srcPtr) & 0x1F]);
							gPart = linePtr->xCLUTG[((*srcPtr) >> 5) & 0x1F];
							rPart = linePtr->xCLUTR[(*srcPtr) >> 10 & 0x1F];
						}
						*destPtr++ = bPart;
						*destPtr++ = gPart;
						*destPtr++ = rPart;

						destPtr += pointlessAlphaByte;
						srcPtr++;

						if (line < newCrop.Top)
							if (!(rPart < 0xF && gPart < 0xF && rPart < 0xF))
								newCrop.Top = line;

						if (pix < newCrop.Left )
							if (!(rPart < 0xF && gPart < 0xF && rPart < 0xF))
								newCrop.Left = pix;

						if (pix > copyWidth - newCrop.Right - 1)
							if (!(rPart < 0xF && gPart < 0xF && rPart < 0xF))
								newCrop.Right = copyWidth - pix - 1;

						if (line > copyHeight - newCrop.Bottom - 1)
							if (!(rPart < 0xF && gPart < 0xF && rPart < 0xF))
								newCrop.Bottom = copyHeight - line - 1;
					}
					if (addBlackBorder)
					{
						// Add a black pixel border (on the right)
						*destPtr++ = 0;
						*destPtr++ = 0;
						*destPtr++ = 0;
						destPtr += pointlessAlphaByte;
						destPtr += ((3 + pointlessAlphaByte) * (bitmapToPrepare.Width - copyWidth - 1));
					}
				}

				if (addBlackBorder)
				{
					// Add a black pixel border (on the bottom)
					for (int pix = 0; pix < copyWidth + 1; pix++)
					{
						*destPtr++ = 0;
						*destPtr++ = 0;
						*destPtr++ = 0;
						destPtr += pointlessAlphaByte;
					}
				}
			}

			bitmapDefinition.Crop.Top = newCrop.Top;
			bitmapDefinition.Crop.Right = newCrop.Right;
			bitmapDefinition.Crop.Bottom = newCrop.Bottom;
			bitmapDefinition.Crop.Left = newCrop.Left;

			bitmapToPrepare.UnlockBits(bitmapData);
		}
	}
}
