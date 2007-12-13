#include "Main.h"

#include "types.h"
#include "Console.h"

IMPLEMENT_APP(FourDOApp)

//////////////////////////////
// Application startup
bool FourDOApp::OnInit()
{
	wxFrame *frame = new wxFrame ((wxFrame*) NULL, -1, _T("4DO"));
	frame->SetIcon (wxIcon(kill_icon_xpm));
	frame->CreateStatusBar ();
	frame->SetStatusText (_T("4DO - HLE 3DO Emulator"));
	frame->SetSize (640, 480);
	frame->Show (TRUE);
	
	SetTopWindow (frame);
	
	return true;
}