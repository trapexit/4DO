#include "Main.h"
#include <iostream>

#include "types.h"
#include "Console.h"

IMPLEMENT_APP(FourDOApp)

/////////////////////////////////////////////////////////////////////////
// Menu items.
/////////////////////////////////////////////////////////////////////////
enum Menu
{
   ID_MENU_FILE_OPENISO = 1,
   ID_MENU_FILE_EXIT,
   ID_MENU_TOOLS_BROWSEISO,
   ID_MENU_HELP_ABOUT,
   
   ID_BUTTON
};

BEGIN_EVENT_TABLE(FourDOApp, wxApp)
   EVT_MENU(ID_MENU_FILE_OPENISO, FourDOApp::OnMenuFileOpenISO)
   EVT_MENU(ID_MENU_FILE_EXIT, FourDOApp::OnMenuFileExit)
   EVT_MENU(ID_MENU_TOOLS_BROWSEISO, FourDOApp::OnMenuToolsBrowseISO)
   EVT_MENU(ID_MENU_HELP_ABOUT, FourDOApp::OnMenuHelpAbout)
END_EVENT_TABLE()

/////////////////////////////////////////////////////////////////////////
// Application startup
/////////////////////////////////////////////////////////////////////////

bool FourDOApp::OnInit()
{
	// TODO: Find out how to load and use command-line arguments.
	
	wxFrame* main = new wxFrame ((wxFrame*) NULL, -1, _T("4DO"));
   InitializeMenu (main);	
	main->SetIcon (wxIcon(kill_icon_xpm));
	main->CreateStatusBar ();
	main->SetStatusText (_T("4DO: Open-Source HLE 3DO Emulator"));
	main->SetSize (640, 480);
	main->CenterOnScreen ();
	main->SetBackgroundColour (wxColor (0xFF000000));
	
   wxGridSizer *sizer = new wxGridSizer (1, 2, 0, 0);
   main->SetSizer (sizer);

   sizer->Add (new wxPanel (main), 2, 0, 0);
   wxTextCtrl* txtBox = new wxTextCtrl (main, -1, wxEmptyString, wxDefaultPosition, wxDefaultSize, wxTE_MULTILINE);
   sizer->Add (txtBox, 2, wxEXPAND, 0);
   
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
   mnuFile->Append (ID_MENU_FILE_OPENISO, _T("&Open ISO...\tCtrl+O"));
   mnuFile->AppendSeparator ();
   mnuFile->Append (ID_MENU_FILE_EXIT, _T("&Exit\tCtrl+X"));

   //////////////////////
   // Tools menu
   mnuMain->Append (mnuTools, _T("&Tools"));
   mnuTools->Append (ID_MENU_TOOLS_BROWSEISO, _T("&Browse ISO...\tCtrl+B"));
   
   //////////////////////
   // Help menu
   mnuMain->Append (mnuHelp, _T("&Help"));
   mnuHelp->Append (ID_MENU_HELP_ABOUT, _T("&About...\tShift+F1"));
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
   wxMessageBox (_T("FourDO - An Open-Source HLE 3DO Emulator\n\nVersion 0.0.0.1"), _T("About 4DO"), wxOK | wxICON_INFORMATION);
}