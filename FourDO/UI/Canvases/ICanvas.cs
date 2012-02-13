using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FourDO.UI.Canvases
{
    interface ICanvas : IDisposable
    {
        bool ImageSmoothing { get; set; }
        bool RenderHighResolution { get; set; }
        bool IsInResizeMode { get; set; }

        void PushFrame(IntPtr currentFrame);

        void Initialize();
        void Destroy();
    }
}
