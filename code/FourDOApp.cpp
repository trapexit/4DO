#include "FourDOApp.h"

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
   wxFrame* main = new MainFrame ();
   main->Show ();
   SetTopWindow (main);
   return true;
}