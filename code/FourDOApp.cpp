#include "FourDOApp.h"
#include "wx/log.h"

IMPLEMENT_APP(FourDOApp)

/////////////////////////////////////////////////////////////////////////
// Application startup
/////////////////////////////////////////////////////////////////////////

int main(int argc, char* argv[])
{
	return WinMain(::GetModuleHandle(NULL), NULL, ::GetCommandLine(), SW_SHOWNORMAL);
}

bool FourDOApp::OnInit()
{
   // 
   // logger
   // 
   wxLog::SetActiveTarget(new wxLogStderr(fopen("fourdo.log", "a")));
   wxLogMessage("=====================================================");

   wxFrame* main = new MainFrame ();
   main->Show ();
   SetTopWindow (main);
   return true;
}
