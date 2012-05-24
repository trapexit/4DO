using System.Runtime.InteropServices;
namespace FourDO.Emulation.FreeDO
{
	public unsafe struct VDLFrame
	{
		public fixed byte lines[(240 * 4) *  // This was the original intended size of this array.
			// The rest are the components of VDLLine... *sigh*
			(
				2 * 320*4 + // line
				1 * 32    + // xCLUTB
				1 * 32    + // xCLUTG
				4         + // xOUTCONTROLL
				4         + // xCLUTDMA
				4           // xBACKGROUND
			)
			];
		public uint srcw;
		public uint srch;
	}

	public unsafe struct VDLLine
	{
		public fixed ushort line[320 * 4];
		public fixed byte xCLUTB[32];
		public fixed byte xCLUTG[32];
		public fixed byte xCLUTR[32];
		public uint xOUTCONTROLL;
		public uint xCLUTDMA;
		public uint xBACKGROUND;
	}
}
