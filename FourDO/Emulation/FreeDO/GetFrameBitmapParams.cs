using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace FourDO.Emulation.FreeDO
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct GetFrameBitmapParams
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
