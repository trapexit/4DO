#include "Main.h"

#include "types.h"
#include "Console.h"

IMPLEMENT_APP(FourDOApp)

/////////////////////////////////////////////////////////////////////////
// Menu items.
/////////////////////////////////////////////////////////////////////////
enum Menu
{
   Menu_File_OpenISO = 1,
   Menu_File_Exit = 1,
   Menu_Tools_BrowseISO,
   Menu_Help_About
};

BEGIN_EVENT_TABLE(FourDOApp, wxApp)
   EVT_MENU(Menu_File_OpenISO, FourDOApp::OnMenuFileOpenISO)
   EVT_MENU(Menu_File_Exit, FourDOApp::OnMenuFileExit)
   EVT_MENU(Menu_Tools_BrowseISO, FourDOApp::OnMenuToolsBrowseISO)
   EVT_MENU(Menu_Help_About, FourDOApp::OnMenuHelpAbout)
END_EVENT_TABLE()


/////////////////////////////////////////////////////////////////////////
// Application startup
/////////////////////////////////////////////////////////////////////////
bool FourDOApp::OnInit()
{
	// TODO: Find out how to load and use command-line arguments.
	
	wxFrame* main = new wxFrame ((wxFrame*) NULL, -1, _T("4DO"));
	main->SetIcon (wxIcon(kill_icon_xpm));
	main->CreateStatusBar ();
	main->SetStatusText (_T("4DO: Open-Source HLE 3DO Emulator"));
	main->SetSize (640, 480);
   InitializeMenu (main);	
	main->CenterOnScreen ();
	main->Show (TRUE);
	
	SetTopWindow (main);
	
	return true;
}

void FourDOApp::InitializeMenu (wxFrame* frame)
{
   wxMenuBar* mnuMain = new wxMenuBar ();
   wxMenu*    mnuFile = new wxMenu ();
   wxMenu*    mnuTools = new wxMenu ();
   wxMenu*    mnuHelp = new wxMenu ();
   
   frame->SetMenuBar (mnuMain);
   
   //////////////////////
   // File menu
   mnuMain->Append (mnuFile, _T("&File"));
   mnuFile->Append (Menu_File_OpenISO, _T("&Open ISO...\tCtrl+O"));
   mnuFile->AppendSeparator ();
   mnuFile->Append (Menu_File_Exit, _T("&Exit\tCtrl+X"));

   //////////////////////
   // Tools menu
   mnuMain->Append (mnuTools, _T("&Tools"));
   mnuTools->Append (Menu_Tools_BrowseISO, _T("&Browse ISO...\tCtrl+B"));
   
   //////////////////////
   // Help menu
   mnuMain->Append (mnuHelp, _T("&Help"));
   mnuHelp->Append (Menu_Help_About, _T("&About...\tShift+F1"));
}

/////////////////////////////////////////////////////////////////////////
// Event handlers
/////////////////////////////////////////////////////////////////////////
void FourDOApp::OnMenuFileOpenISO (wxCommandEvent& WXUNUSED(event))
{
   wxString fileName = wxFileSelector (_T("Open 3DO ISO File"), NULL, NULL, NULL, _T("ISO Files (*.iso)|*.iso|All files (*.*)|*.*"), wxFD_OPEN | wxFD_FILE_MUST_EXIST);
   if (!fileName.empty())
   {
       wxMessageBox (_T("This will load an ISO"));
   }
}

void FourDOApp::OnMenuFileExit (wxCommandEvent& WXUNUSED(event))
{
   this->Exit ();
}

void FourDOApp::OnMenuToolsBrowseISO (wxCommandEvent& WXUNUSED(event))
{
   wxString fileName = wxFileSelector (_T("Open 3DO ISO File"), NULL, NULL, NULL, _T("ISO Files (*.iso)|*.iso|All files (*.*)|*.*"), wxFD_OPEN | wxFD_FILE_MUST_EXIST);
   if (!fileName.empty())
   {
       wxMessageBox (_T("This will load the ISO Browser"));
   }
}

void FourDOApp::OnMenuHelpAbout (wxCommandEvent& WXUNUSED(event))
{
   wxMessageBox (_T("FourDO - An Open-Source HLE 3DO Emulator\r\n\r\nVersion 0.0.0.1"), _T("About 4DO"), wxOK | wxICON_INFORMATION);
}