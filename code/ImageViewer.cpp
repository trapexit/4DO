#include "ImageViewer.h"

/////////////////////////////////////////////////////////////////////////
// Event declaration
/////////////////////////////////////////////////////////////////////////
BEGIN_EVENT_TABLE(ImageViewer, wxFrame)
    EVT_PAINT(ImageViewer::onPaint)
END_EVENT_TABLE()

/////////////////////////////////////////////////////////////////////////
// Frame startup
/////////////////////////////////////////////////////////////////////////
ImageViewer::ImageViewer(wxFrame* parent, wxString isoPath, wxString filePath)
      : wxFrame (parent, -1, wxEmptyString, wxDefaultPosition, wxDefaultSize)
{
	const uint32_t headerLength = 36;
	uint32_t       bytesRead;
	uint8_t        header[headerLength];
	bool           ret;

	this->SetTitle (wxString().Format ("BMP test - %s", isoPath));

	/////////////////////////
   
	File f(isoPath);

	ret = f.openFile(filePath.c_str());

	if (!ret)
	{
		wxMessageBox(wxString::Format("Couldn't open %s for display", filePath));
		return;
	}

	f.read(header, headerLength, &bytesRead);

	bmpLength = f.getFileSize() - headerLength;
	bmp = new uint8_t[bmpLength];

	f.read(bmp, bmpLength, &bytesRead);

	width  = 640;
	height = 120;

	// I'm doing this for the resize handle.
	this->SetStatusBar (new wxStatusBar (this));
   
	this->SetSize ((320 * 2) + 10, (240 * 2) + 45);
	this->CenterOnScreen ();
}

ImageViewer::~ImageViewer()
{
}

void ImageViewer::onPaint(wxPaintEvent &WXUNUSED(event))
{
	int     i = 0;
	wxImage image(width, height * 2);

	for (int y = 0; y < height; y++)
	{
		for (int x = 0; x < width; x++)
		{
		   uint8_t r;
		   uint8_t g;
		   uint8_t b;
		   
		   // 0RRRRRGGGGGBBBBB
			r = (bmp[i] >> 2) & 0x1F;
			g = ((bmp[i] & 0x03) << 3) | (bmp[i + 1] >> 5);
			b = bmp[i + 1] & 0x1F;
   		
   		if (x % 2 == 0)
   		{
   		   image.SetRGB(x, y * 2, r * 8, g * 8, b * 8);
   		   image.SetRGB(x + 1, y * 2, r * 8, g * 8, b * 8);
   		}
   		else
   		{
   		   image.SetRGB(x - 1, y * 2 + 1, r * 8, g * 8, b * 8);
   		   image.SetRGB(x, y * 2 + 1, r * 8, g * 8, b * 8);
   		}
         
			i += 2;
		}
	}

	wxBitmap bitmap(image.Scale(320 * 2, 240 * 2));

	wxPaintDC dc( this );
	dc.DrawBitmap( bitmap, 0, 0, true /* use mask */ );
}
