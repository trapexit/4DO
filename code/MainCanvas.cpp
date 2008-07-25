#include <wx/wx.h>
#include "MainCanvas.h"

MainCanvas::MainCanvas( wxWindow* parent, wxWindowID id, uchar* ramPointer )
      : wxPanel( parent, id, wxDefaultPosition, wxSize(-1, 30), wxNO_BORDER )
{
	bmp = ramPointer;
	bmpLength = 0x00025834;

	m_width  = 640;
	m_height = 120;
	
	m_image = new wxImage (m_width, m_height * 2);
	Connect( wxEVT_PAINT, wxPaintEventHandler(MainCanvas::OnPaint ) );
	
	this->UpdateImage();
}

MainCanvas::~MainCanvas()
{
	delete m_image;
}

void MainCanvas::UpdateImage()
{
	int     i = 0;

	for (int y = 0; y < m_height; y++)
	{
		for (int x = 0; x < m_width; x++)
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
   		   m_image->SetRGB(x, y * 2, r * 8, g * 8, b * 8);
   		   m_image->SetRGB(x + 1, y * 2, r * 8, g * 8, b * 8);
   		}
   		else
   		{
   		   m_image->SetRGB(x - 1, y * 2 + 1, r * 8, g * 8, b * 8);
   		   m_image->SetRGB(x, y * 2 + 1, r * 8, g * 8, b * 8);
   		}
         
			i += 2;
		}
	}
}

void MainCanvas::OnPaint( wxPaintEvent& event )
{
	wxPaintDC   dc( this );
	wxBitmap    bitmap( m_image->Scale( 320 * 2, 240 * 2 ) );
	
	dc.DrawBitmap( bitmap, 0, 0, true );
}