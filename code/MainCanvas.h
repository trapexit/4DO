#ifndef MAINCANVAS_H
#define MAINCANVAS_H

#include <wx/wx.h>

#include "types.h"

class MainCanvas : public wxPanel
{
public:
	MainCanvas( wxWindow* parent, wxWindowID id, uchar* ramPointer  );
	~MainCanvas();

	void UpdateImage();

	void OnSize( wxSizeEvent& event );
	void OnPaint( wxPaintEvent& event );
	
private:
	uchar*    bmp;
	uint      bmpLength;
	
	int       m_width;
	int 	  m_height;
	wxImage*  m_image;
};

#endif
