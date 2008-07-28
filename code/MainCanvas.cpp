#include <wx/wx.h>
#include "MainCanvas.h"

#define NATIVE_WDTH 320
#define NATIVE_HGHT 240

#define SCREEN_CURRENT m_bitmap[ m_currentScreen ];

MainCanvas::MainCanvas( wxWindow* parent, wxWindowID id, uchar* ramPointer )
      : wxPanel( parent, id, wxDefaultPosition, wxSize(-1, 30), wxNO_BORDER )
{
	m_currentScreen = 0;
	
	bmp = ramPointer;
	bmpLength = 0x00025834;

	m_image = new wxImage (NATIVE_WDTH, NATIVE_HGHT);
	Connect( wxEVT_ERASE_BACKGROUND, wxEraseEventHandler(MainCanvas::OnErase ) );
	Connect( wxEVT_PAINT,            wxPaintEventHandler(MainCanvas::OnPaint ) );
	Connect( wxEVT_SIZE,             wxSizeEventHandler (MainCanvas::OnSize  ) );
	
	m_bitmap[ 0 ] = new wxBitmap( NATIVE_WDTH, NATIVE_HGHT, -1 );
	m_bitmap[ 1 ] = new wxBitmap( NATIVE_WDTH, NATIVE_HGHT, -1 );
	
	this->UpdateImage();
}

MainCanvas::~MainCanvas()
{
	delete m_image;
	delete m_bitmap[ 0 ];
	delete m_bitmap[ 1 ];
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

	if( width <= 0 || height <= 0 )
		return;

	image = m_image->Scale( width, height, wxIMAGE_QUALITY_NORMAL );
	//image = m_image->Scale( width, height, wxIMAGE_QUALITY_HIGH );
	//image = m_image->Scale( NATIVE_WDTH, NATIVE_HGHT, wxIMAGE_QUALITY_NORMAL );
	//image = m_image->Scale( NATIVE_WDTH * 2, NATIVE_HGHT * 2, wxIMAGE_QUALITY_NORMAL );
	
	if( m_currentScreen == 0 )
	{
		delete m_bitmap[ 1 ];
		m_bitmap[ 1 ] = new wxBitmap( image, -1 );
		m_currentScreen = 1;
	}
	else
	{
		delete m_bitmap[ 0 ];
		m_bitmap[ 0 ] = new wxBitmap( image, -1 );
		m_currentScreen = 0;
	}
}

void MainCanvas::OnPaint( wxPaintEvent& event )
{
	int       width;
	int       height;
	wxBrush   brush;
	wxPen     pen;
	wxBitmap* bitmap;
	
	this->GetSize( &width, &height );

	// Create DC for canvas (required)
	wxPaintDC   dc( this );
	
	// Get the size of our window.
	this->GetSize( &width, &height );
	
	// Draw whatever bitmap we have stored.
	bitmap = m_bitmap[ m_currentScreen ];
	dc.DrawBitmap( *bitmap, 0, 0, true );
	
	// Draw gray on the rest.
	brush = *wxLIGHT_GREY_BRUSH;
	pen = *wxLIGHT_GREY_PEN;
	
	dc.SetBrush ( brush );
	dc.SetPen ( pen );
	dc.DrawRectangle( bitmap->GetWidth (), 0, width - bitmap->GetWidth(), height );
	dc.DrawRectangle( 0,  bitmap->GetHeight (), width, height - bitmap->GetHeight() );
}

void MainCanvas::OnSize( wxSizeEvent& event )
{
	this->UpdateBitmap();
}

void MainCanvas::OnErase( wxEraseEvent& event )
{
	// We don't want to erase!
}