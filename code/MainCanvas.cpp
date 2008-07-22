#include <wx/wx.h>
#include "MainCanvas.h"

MainCanvas::MainCanvas( wxWindow* parent, wxWindowID id, uchar* ramPointer )
      : wxPanel( parent, id, wxDefaultPosition, wxSize(-1, 30), wxNO_BORDER )
{
	bmp = ramPointer;
	bmpLength = 0x00025834;

	width  = 640;
	height = 120;
	
	Connect( wxEVT_PAINT, wxPaintEventHandler(MainCanvas::OnPaint ) );
	Connect( wxEVT_SIZE, wxSizeEventHandler(MainCanvas::OnSize ) );
}

void MainCanvas::OnPaint( wxPaintEvent& event )
{
	int     i = 0;
	wxImage image(width, height * 2);

	for (int y = 0; y < height; y++)
	{
		for (int x = 0; x < width; x++)
		{
		   uchar r;
		   uchar g;
		   uchar b;
		   
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
	dc.DrawBitmap( bitmap, 0, 0, true );
}

void MainCanvas::OnSize(wxSizeEvent& event)
{
	this->Refresh();
}
