#include "ImageTest.h"

/////////////////////////////////////////////////////////////////////////
// Event declaration
/////////////////////////////////////////////////////////////////////////
BEGIN_EVENT_TABLE(ImageTest, wxFrame)
    EVT_PAINT(ImageTest::onPaint)
END_EVENT_TABLE()

/////////////////////////////////////////////////////////////////////////
// Frame startup
/////////////////////////////////////////////////////////////////////////
ImageTest::ImageTest(wxFrame* parent)
      : wxFrame (parent, -1, wxEmptyString, wxDefaultPosition, wxDefaultSize)
{
	bool           ret;

	this->SetTitle (wxString().Format("BMP test"));

	DMA = new DMAController();
	int address = 0x002C0000;
	int color = 0;

	while (address < 0x002e57ff)
	{
		color += 5;
		DMA->SetByte(address, (uchar) color);
		address++;
		if (color == 255)
		{
			color = 0;
		}
	}

	// I'm doing this for the resize handle.
	this->SetStatusBar (new wxStatusBar (this));
   
	this->SetSize (320 + 10, 240 + 45);
	this->CenterOnScreen ();
}

ImageTest::~ImageTest()
{
}

void ImageTest::onPaint(wxPaintEvent &WXUNUSED(event))
{
	int     width = 320, height = 240;
	unsigned int address = 0x002bffff;
	wxImage image(width, height);

	for (int y = 0; y < height; y++)
	{
		for (int x = 0; x < width; x++)
		{
			int byte1 = DMA->GetWord(address++);
			int byte2 = DMA->GetWord(address++);
			
			// 0RRRRRGGGGGBBBBB
			int r = (byte1 >> 2) & 0x1F;
			int g = ((byte1 & 0x03) << 3) | (byte2 >> 5);
			int b = byte2 & 0x1F;

		   image.SetRGB(x, y, r * 8, g * 8, b * 8);
   		
		/*
			if (x % 2 == 0)
			{
			   image.SetRGB(x, y, r * 8, g * 8, b * 8);
			   image.SetRGB(x + 1, y, r * 8, g * 8, b * 8);
			}
			else
			{
			   image.SetRGB(x - 1, y + 1, r * 8, g * 8, b * 8);
			   image.SetRGB(x, y + 1, r * 8, g * 8, b * 8);
			}
		*/
		}
	}

	wxBitmap bitmap(image);

	wxPaintDC dc( this );
	dc.DrawBitmap( bitmap, 0, 0, true /* use mask */ );
}
