#ifndef MAINCANVAS_H
#define MAINCANVAS_H

#include <wx/wx.h>

#include "types.h"

class MainCanvas : public wxPanel
{
public:
	MainCanvas( wxWindow* parent, wxWindowID id, uchar* ramPointer  );

	void OnSize( wxSizeEvent& event );
	void OnPaint( wxPaintEvent& event );

   
private:
	uchar*   bmp;
	uint     bmpLength;
	int      width;
	int 	 height;
};

#endif
