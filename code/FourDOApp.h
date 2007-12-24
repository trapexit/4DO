#ifndef _INC_FOURDOAPP
#define _INC_FOURDOAPP

#include "wx/wx.h"
#include "MainFrame.h"
#include "types.h"
#include "Console.h"

class FourDOApp : public wxApp
{
public:
   virtual bool OnInit();
};

DECLARE_APP(FourDOApp)

#endif //_INC_FOURDOAPP