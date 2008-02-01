#include "FourDOApp.h"
#include "wx/log.h"

IMPLEMENT_APP(FourDOApp)

/////////////////////////////////////////////////////////////////////////
// Application startup
/////////////////////////////////////////////////////////////////////////

int main()
{
	return WinMain(::GetModuleHandle(NULL), NULL, ::GetCommandLine(), SW_SHOWNORMAL);
}

bool FourDOApp::OnInit()
{
   //
   // Even though we're a windowed app, I'm still redirecting output to the command line
   // (If I don't do this, command line usage summary shows up in a message box... yuck)
   //
   wxMessageOutput::Set (new wxMessageOutputStderr ());

   // 
   // Command line parser.
   // 
   wxString  logo;
   wxCmdLineParser parser(this->argc, this->argv);
   
   parser.SetSwitchChars ("-/");

   logo.Append ("============================================================\r\n");
   logo.Append ("FourDO - An open-source HLE 3DO Emulator\r\n");
   logo.Append ("============================================================\r\n");
   parser.AddSwitch ("?", "", "Displays usage information", wxCMD_LINE_PARAM_OPTIONAL | wxCMD_LINE_OPTION_HELP);
   parser.AddSwitch ("h", "help", "Displays usage information", wxCMD_LINE_PARAM_OPTIONAL | wxCMD_LINE_OPTION_HELP);
   parser.AddSwitch ("d", "debug", "Enables debug mode", wxCMD_LINE_PARAM_OPTIONAL);
   parser.AddOption ("li", "loadimage", "Loads a CD image file", wxCMD_LINE_VAL_STRING, wxCMD_LINE_PARAM_OPTIONAL);
   parser.AddOption ("lc", "loadcode", "Loads a file as ARM60 code", wxCMD_LINE_VAL_STRING, wxCMD_LINE_PARAM_OPTIONAL);
   parser.SetLogo (logo);
   parser.EnableLongOptions ();

   // 
   // logger
   // 
   wxLog::SetActiveTarget(new wxLogStderr(fopen("fourdo.log", "a")));
   wxLogMessage("=====================================================");

   if (parser.Parse () == 0)
   {
      wxFrame* main = new MainFrame (&parser);
      main->Show ();
      SetTopWindow (main);
      return true;
   }
   else
   {
      // Cancel out.
      return false;
   }
}