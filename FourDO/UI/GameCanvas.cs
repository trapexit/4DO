// TODO: Currently, the invalidate rect does the whole screen. It could technically do just the rectangle including the image to blit.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FourDO.Emulation;
using FourDO.Emulation.FreeDO;

namespace FourDO.UI
{
    public partial class GameCanvas : UserControl
    {
        private readonly bool preserveAspectRatio = false;

        private const int bitmapWidth = 640;
        private const int bitmapHeight = 480;

        private Bitmap bitmapA = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format24bppRgb);
        private Bitmap bitmapB = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format24bppRgb);
        private Bitmap bitmapC = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format24bppRgb);
        
        private Bitmap currentFrontendBitmap;
        private Bitmap lastDrawnBackgroundBitmap;
        
        private object bitmapSemaphore = new object();

        public GameCanvas()
        {
            InitializeComponent();

            currentFrontendBitmap = bitmapA;
            lastDrawnBackgroundBitmap = bitmapB;

            GameConsole.Instance.FrameDone += new EventHandler(Instance_FrameDone);
        }

        private unsafe void Instance_FrameDone(object sender, EventArgs e)
        {
            /////////////// 
            // Choose the best bitmap to do a background render to
            Bitmap bitmapToCalc;
            lock (bitmapSemaphore)
            {
                if ((bitmapA != currentFrontendBitmap) && (bitmapA != lastDrawnBackgroundBitmap))
                    bitmapToCalc = bitmapA;
                else if ((bitmapB != currentFrontendBitmap) && (bitmapB != lastDrawnBackgroundBitmap))
                    bitmapToCalc = bitmapB;
                else
                    bitmapToCalc = bitmapC;
            }

            int frameNum = (bitmapToCalc == bitmapA) ? 1 : 2;

            int bitmapHeight = bitmapToCalc.Height;
            int bitmapWidth = bitmapToCalc.Width;
            BitmapData bitmapData = bitmapToCalc.LockBits(new Rectangle(0, 0, bitmapToCalc.Width, bitmapToCalc.Height), ImageLockMode.WriteOnly, bitmapToCalc.PixelFormat);
            int bitmapStride = bitmapData.Stride;

            byte* destPtr = (byte*)bitmapData.Scan0.ToPointer();
            VDLFrame* framePtr = (VDLFrame*)GameConsole.Instance.CurrentFrame.ToPointer();
            for (int line = 0; line < bitmapHeight; line++)
            {
                VDLLine* linePtr = (VDLLine*)&(framePtr->lines[sizeof(VDLLine) * line]);
                short* srcPtr = (short*)linePtr;
                for (int pix = 0; pix < bitmapWidth; pix++)
                {
                    *destPtr++ = (byte)(linePtr->xCLUTG[(*srcPtr) & 0x1F]);
                    *destPtr++ = linePtr->xCLUTG[((*srcPtr) >> 5) & 0x1F];
                    *destPtr++ = linePtr->xCLUTR[(*srcPtr) >> 10 & 0x1F];
                    srcPtr++;
                }
            }

            bitmapToCalc.UnlockBits(bitmapData);

            lastDrawnBackgroundBitmap = bitmapToCalc;
            
            this.Invalidate();
        }

        private void GameCanvas_Paint(object sender, PaintEventArgs e)
        {
            currentFrontendBitmap = lastDrawnBackgroundBitmap;

            double imageAspect = bitmapWidth / (double)bitmapHeight;
            double screenAspect = this.Width / (double)this.Height;

            Rectangle blitRect = new Rectangle();

            if (preserveAspectRatio == true)
            {
                blitRect.X = 0;
                blitRect.Y = 0;
                blitRect.Width = this.Width;
                blitRect.Height = this.Height;
            }
            else
            {
                if (screenAspect > imageAspect)
                {
                    // window is wider than it should be.
                    blitRect.Width = (int)(bitmapWidth * (this.Height / (double)bitmapHeight));
                    blitRect.Height = this.Height;
                    blitRect.X = (this.Width - blitRect.Width) / 2;
                    blitRect.Y = 0;
                }
                else
                {
                    blitRect.Width = this.Width;
                    blitRect.Height = (int)(bitmapHeight * (this.Width / (double)bitmapWidth));
                    blitRect.X = 0;
                    blitRect.Y = (this.Height - blitRect.Height) / 2;
                }
            }
            Graphics g = e.Graphics;
            g.DrawImage(currentFrontendBitmap, blitRect);
            g.DrawRectangle(new Pen(Color.FromArgb(50,50,50)), blitRect.X - 1, blitRect.Y - 1, blitRect.Width + 1, blitRect.Height + 1);
            
        }
    }
}