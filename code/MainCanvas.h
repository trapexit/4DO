#ifndef MAINCANVAS_H
#define MAINCANVAS_H

#include <wx/wx.h>

#include "types.h"

#define SCREEN_COUNT 2

class MainCanvas : public wxPanel
{
public:
	MainCanvas( wxWindow* parent, wxWindowID id, uchar* ramPointer  );
	~MainCanvas();

	void UpdateImage();

	void OnPaint( wxPaintEvent& event );
	void OnSize( wxSizeEvent& event );
	void OnErase( wxEraseEvent& event );
	
private:
	void UpdateBitmap();
	
	uchar*    bmp;
	uint      bmpLength;
	
	wxImage*  m_image;
	
	wxBitmap* m_bitmap[ SCREEN_COUNT ];
	int       m_currentScreen;
};

#endif
