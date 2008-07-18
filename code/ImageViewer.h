#ifndef _IMAGEVIEWER_H_
#define _IMAGEVIEWER_H_

#include "wx/wx.h"
#include "wx/image.h"

#include "filesystem\file.h"

class ImageViewer : public wxFrame
{
public:
   ImageViewer(wxFrame* parent,                   uint8_t*   ramPointer);
   ImageViewer(wxFrame* parent, wxString isoPath, wxString filePath);
   ~ImageViewer();
   
   void onPaint(wxPaintEvent &event);
private:
	uint8_t  *bmp;
	uint32_t bmpLength;

	uint32_t width, height;

	DECLARE_EVENT_TABLE();
};

#endif // _IMAGEVIEWER_H_
