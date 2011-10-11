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
            BinaryWriter writer = null;
            try
            {
                using (writer = new BinaryWriter(new FileStream(fileName, FileMode.Create)))
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
                if (writer != null)
                    writer.Close();
                return false;
            }
            return true;
        }

        public static unsafe bool ReadMemoryDump(byte[] targetMemoryBuffer, string fileName, int length)
        {
            BinaryReader reader = null;
            try
            {
                using (reader = new BinaryReader(new FileStream(fileName, FileMode.Open)))
                {
                    reader.Read(targetMemoryBuffer, 0, length);
                }
            }
            catch
            {
                if (reader != null)
                    reader.Close(); 
                return false;
            }
            return true;
        }
    }
}
