#ifndef _IMAGETEST_H_
#define _IMAGETEST_H_

#include "wx/wx.h"
#include "wx/image.h"
#include "DMAController.h"

class ImageTest : public wxFrame
{
public:
   ImageTest(wxFrame* parent);
   ~ImageTest();
   
   void onPaint(wxPaintEvent &event);
private:
	int width, height;
	DMAController *DMA;

	DECLARE_EVENT_TABLE();
};

#endif // _IMAGETEST_H_
