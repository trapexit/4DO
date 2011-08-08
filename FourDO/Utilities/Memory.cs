using System;
using System.IO;
using System.Runtime.InteropServices;

namespace FourDO.Utilities
{
    public static class Memory
    {
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void InternalCopyMemory(IntPtr destination, IntPtr source, uint length);

        public static void CopyMemory(IntPtr destination, IntPtr source, int length)
        {
            InternalCopyMemory(destination, source, (uint)length);
        }

        public static unsafe bool WriteMemoryDump(string fileName, IntPtr memoryLocation, int length)
        {
            try
            {
                using (BinaryWriter writer = new BinaryWriter(new FileStream(fileName, FileMode.Create)))
                {
                    byte* memPtr = (byte*)memoryLocation.ToPointer();
                    for (int byteNum = 0; byteNum < length; byteNum++)
                    {
                        writer.Write(*memPtr++);
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
