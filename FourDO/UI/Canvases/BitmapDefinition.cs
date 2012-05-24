using System.Drawing;
using System.Drawing.Imaging;
using FourDO.Emulation.FreeDO;

namespace FourDO.UI.Canvases
{
	internal class BitmapDefinition
	{
		public BitmapDefinition(int bitmapWidth, int bitmapHeight, PixelFormat format)
		{
			this.Bitmap = new Bitmap(bitmapWidth, bitmapHeight, format);
			this.Crop = new BitmapCrop();
		}

		public BitmapCrop Crop { get; set; }
		public Bitmap Bitmap { get; set; }
	}
}
