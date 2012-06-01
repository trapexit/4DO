using System;
using System.Runtime.InteropServices;

namespace FourDO.Emulation.FreeDO
{
	public enum ScalingAlgorithm
	{
		None = 0,
		Hq2X = 1,
		Hq3X = 2,
		Hq4X = 3
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class GetFrameBitmapParams
	{
		public IntPtr sourceFrame;
		public IntPtr destinationBitmap;
		public int destinationBitmapWidthPixels;
		public IntPtr bitmapCrop;
		public int copyWidthPixels;
		public int copyHeightPixels;
		public byte addBlackBorder;
		public byte copyPointlessAlphaByte;
		public byte allowCrop;
		public int scalingAlgorithm;
		public int resultingWidth;
		public int resultingHeight;
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class GetFrameBitmapCrop
	{
		public int left;
		public int top;
		public int bottom;
		public int right;
	}
}
