//////////////////////////////////////////////////////////
// JMK NOTES:
// I only spent one night getting this hooked up. It was relatively easy to figure out the
// right sample rate just by listening to the resulting audio.
//
// I've used a libary from here to play the audio from a buffer
// http://www.codeproject.com/KB/audio-video/cswavplay.aspx
// (consent is pending, I've emailed the guy... maybe the email address is too old?)
//
// Things to do:
// * I just blindly selected BUFFER SIZES. This is pretty lazy of me. I believe it introduces audio lag too.
// * If I understand things right, copying bytes around could be FASTER by using RtlMoveMemory from kernel32.
// * Audio plugins will suffer if the system doesn't stay at 60fps internally. It's possible the audio plugins
//   should be made aware of these situations so it can do something. Though I don't know what it could do.
//////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaveLib;

namespace FourDO.Emulation.Plugins.Audio
{
    internal class DefaultAudioPlugin : IAudioPlugin
    {
        private const int bytesPerSample = 4;
        
        private WaveOutPlayer player;
        private WaveFormat format;

        private byte[] internalBuffer;
        private int internalPosition;
        private Stream internalReadStream;

        internal DefaultAudioPlugin()
        {
            format = new WaveFormat(88200, 8 * bytesPerSample, 1);
            player = new WaveOutPlayer(-1, format, 16384, 3, new WaveLib.BufferFillEventHandler(this.FillerCallback));

            internalBuffer = new byte[16384 * 3 * bytesPerSample];
            internalPosition = 0;
            internalReadStream = new MemoryStream(internalBuffer);
        }

        #region IAudioPlugin Implementation

        public bool GetHasSettings()
        {
            return false;
        }

        public void ShowSettings(IWin32Window owner)
        {
            return;
        }

        public void PushSample(uint dspSample)
        {
            unsafe
            {
                int bufferLen =internalBuffer.Length;
                fixed (byte* internalBufferPointer = internalBuffer)
                {
                    ((uint*)internalBufferPointer)[internalPosition++] = dspSample;
                    internalPosition++;
                    if (internalPosition >= (bufferLen / bytesPerSample))
                        internalPosition = 0;
                }
            }
        }

        public void Destroy()
        {
            player.Dispose();
        }

        #endregion // IAudioPlugin Implementation

        private void FillerCallback(IntPtr data, int size)
        {
            byte[] b = new byte[size];
            if (internalReadStream != null)
            {
                int pos = 0;
                while (pos < size)
                {
                    int toget = size - pos;
                    int got = internalReadStream.Read(b, pos, toget);
                    if (got < toget)
                        internalReadStream.Position = 0; // loop if the file ends
                    pos += got;
                }
            }
            else
            {
                for (int i = 0; i < b.Length; i++)
                    b[i] = 0;
            }
            System.Runtime.InteropServices.Marshal.Copy(b, 0, data, size);
        }
    }
}
