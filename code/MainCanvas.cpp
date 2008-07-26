#include <wx/wx.h>
#include "MainCanvas.h"

#define NATIVE_WDTH 320
#define NATIVE_HGHT 240

MainCanvas::MainCanvas( wxWindow* parent, wxWindowID id, uchar* ramPointer )
      : wxPanel( parent, id, wxDefaultPosition, wxSize(-1, 30), wxNO_BORDER )
{
	bmp = ramPointer;
	bmpLength = 0x00025834;

	m_image = new wxImage (NATIVE_WDTH, NATIVE_HGHT);
	Connect( wxEVT_ERASE_BACKGROUND, wxEraseEventHandler(MainCanvas::OnErase ) );
	Connect( wxEVT_PAINT,            wxPaintEventHandler(MainCanvas::OnPaint ) );
	Connect( wxEVT_SIZE,             wxSizeEventHandler (MainCanvas::OnSize  ) );
	
	m_bitmap = new wxBitmap( NATIVE_WDTH, NATIVE_HGHT, -1 );
	
	this->UpdateImage();
}

MainCanvas::~MainCanvas()
{
	delete m_image;
	delete m_bitmap;
}

void MainCanvas::UpdateImage()
{
	int x = 0;
	int y = 0;

	uchar r;
	uchar g;
	uchar b;

	for (int i = 0; i < NATIVE_HGHT * NATIVE_WDTH * 2; i+= 2)
	{
		// 0RRRRRGGGGGBBBBB
		r = (bmp[i] >> 2) & 0x1F;
		g = ((bmp[i] & 0x03) << 3) | (bmp[i + 1] >> 5);
		b = bmp[i + 1] & 0x1F;

		m_image->SetRGB(x, y, r * 8, g * 8, b * 8);

		if (i % 4)
		{
			++x;
			--y;
		}
		else
		{
			++y;
		}
		
		if (x == NATIVE_WDTH)
		{
			// Move two lines down.
			x =  0;
			y += 2;
		}
	}
	this->UpdateBitmap();
}

void MainCanvas::UpdateBitmap()
{
	int      width;
	int      height;
	wxImage  image;
	
	this->GetSize( &width, &height );
	//image = m_image->Scale( width, height, wxIMAGE_QUALITY_NORMAL );
	image = m_image->Scale( NATIVE_WDTH * 2, NATIVE_HGHT * 2, wxIMAGE_QUALITY_NORMAL );
	
	m_bitmap = new wxBitmap( image, -1 );
}

void MainCanvas::OnPaint( wxPaintEvent& event )
{
	// Create DC for canvas (required)
	wxPaintDC   dc( this );
	
	// Draw whatever bitmap we have stored.
	dc.DrawBitmap( *m_bitmap, 0, 0, true );
}

void MainCanvas::OnSize( wxSizeEvent& event )
{
	this->UpdateBitmap();
}

void MainCanvas::OnErase( wxEraseEvent& event )
{
	// We don't want to erase!
}