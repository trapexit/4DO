using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
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

		public unsafe static void CopyBitmap(IntPtr currentFrame, Bitmap bitmapToPrepare, int copyWidth, int copyHeight, bool addBlackBorder, bool copyPointlessAlphaByte)
		{
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
						if (*srcPtr == 0)
						{
							*destPtr++ = (byte)(linePtr->xBACKGROUND & 0x1F);
							*destPtr++ = (byte)((linePtr->xBACKGROUND >> 5) & 0x1F);
							*destPtr++ = (byte)((linePtr->xBACKGROUND >> 10) & 0x1F);
						}
						else if (allowFixedClut && (*srcPtr & 0x8000) > 0)
						{
							*destPtr++ = FIXED_CLUTB[(*srcPtr) & 0x1F];
							*destPtr++ = FIXED_CLUTG[((*srcPtr) >> 5) & 0x1F];
							*destPtr++ = FIXED_CLUTR[(*srcPtr) >> 10 & 0x1F];
						}
						else
						{
							*destPtr++ = (byte)(linePtr->xCLUTB[(*srcPtr) & 0x1F]);
							*destPtr++ = linePtr->xCLUTG[((*srcPtr) >> 5) & 0x1F];
							*destPtr++ = linePtr->xCLUTR[(*srcPtr) >> 10 & 0x1F];
						}
						destPtr += pointlessAlphaByte;
						srcPtr++;
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

			bitmapToPrepare.UnlockBits(bitmapData);
		}
	}
}
