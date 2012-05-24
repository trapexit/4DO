#include "freedoconfig.h"
#include "freedocore.h"
#include "frame.h"

unsigned char FIXED_CLUTR[32];
unsigned char FIXED_CLUTG[32];
unsigned char FIXED_CLUTB[32];


void _frame_Init()
{
	for(int j = 0; j < 32; j++)
	{
		FIXED_CLUTR[j] = (unsigned char)(((j & 0x1f) << 3) | ((j >> 2) & 7));
		FIXED_CLUTG[j] = FIXED_CLUTR[j];
		FIXED_CLUTB[j] = FIXED_CLUTR[j];
	}
}

void Get_Frame_Bitmap(
	VDLFrame* sourceFrame,
	void* destinationBitmap,
	int destinationBitmapWidth,
	BitmapCrop* bitmapCrop,
	int copyWidth,
	int copyHeight,
	bool addBlackBorder,
	bool copyPointlessAlphaByte,
	bool allowCrop)
{
	float maxCropPercent = allowCrop ? .25f : 0;
	int maxCropTall = (int)(copyHeight * maxCropPercent);
	int maxCropWide = (int)(copyWidth * maxCropPercent);

	bitmapCrop->top = maxCropTall;
	bitmapCrop->left = maxCropWide;
	bitmapCrop->right = maxCropWide;
	bitmapCrop->bottom = maxCropTall;

	int pointlessAlphaByte = copyPointlessAlphaByte ? 1 : 0;

	byte* destPtr = (byte*)destinationBitmap;
	VDLFrame* framePtr = sourceFrame;
	for (int line = 0; line < copyHeight; line++)
	{
		VDLLine* linePtr = &framePtr->lines[line];
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

			if (line < bitmapCrop->top)
				if (!(rPart < 0xF && gPart < 0xF && bPart < 0xF))
					bitmapCrop->top = line;

			if (pix < bitmapCrop->left )
				if (!(rPart < 0xF && gPart < 0xF && bPart < 0xF))
					bitmapCrop->left = pix;

			if (pix > copyWidth - bitmapCrop->right - 1)
				if (!(rPart < 0xF && gPart < 0xF && bPart < 0xF))
					bitmapCrop->right = copyWidth - pix - 1;

			if (line > copyHeight - bitmapCrop->bottom - 1)
				if (!(rPart < 0xF && gPart < 0xF && bPart < 0xF))
					bitmapCrop->bottom = copyHeight - line - 1;
		}
		if (addBlackBorder)
		{
			// Add a black pixel border (on the right)
			*destPtr++ = 0;
			*destPtr++ = 0;
			*destPtr++ = 0;
			destPtr += pointlessAlphaByte;
			destPtr += ((3 + pointlessAlphaByte) * (destinationBitmapWidth - copyWidth - 1));
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