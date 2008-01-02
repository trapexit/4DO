#include "MainFrame.h"

/////////////////////////////////////////////////////////////////////////
// Menu items.
/////////////////////////////////////////////////////////////////////////
enum Menu
{
   ID_MENU_FILE_OPENISO = 1,
   ID_MENU_FILE_EXIT,
   ID_MENU_TOOLS_BROWSEISO,
   ID_MENU_HELP_ABOUT
};

BEGIN_EVENT_TABLE(MainFrame, wxFrame)
   EVT_MENU(ID_MENU_FILE_OPENISO, MainFrame::OnMenuFileOpenISO)
   EVT_MENU(ID_MENU_FILE_EXIT, MainFrame::OnMenuFileExit)
   EVT_MENU(ID_MENU_TOOLS_BROWSEISO, MainFrame::OnMenuToolsBrowseISO)
   EVT_MENU(ID_MENU_HELP_ABOUT, MainFrame::OnMenuHelpAbout)
END_EVENT_TABLE()

/////////////////////////////////////////////////////////////////////////
// Frame startup
/////////////////////////////////////////////////////////////////////////

MainFrame::MainFrame(wxCmdLineParser* parser)
      : wxFrame((wxFrame *) NULL, -1, wxEmptyString, wxDefaultPosition, wxDefaultSize)
{
   /////////////////////
   // Handle command-line arguments.
   m_isDebug = parser->Found ("d");
   if (parser->Found ("li"))
   {
      parser->Found ("li", &m_fileName);
   }
   
   /////////////////////
   // GUI Setup.
   this->SetTitle ("4DO");
   this->SetIcon (wxIcon(fourdo_xpm));
	this->SetSize (640, 480);
	this->CenterOnScreen ();
	this->SetBackgroundColour (wxColor (0xFF000000));
	this->CreateStatusBar ();
	this->SetStatusText (_T("4DO: Open-Source HLE 3DO Emulator"));
   this->InitializeMenu ();	
	
	// A quick debug textbox.
	grdDebug = new wxGrid (this, -1, wxDefaultPosition, wxDefaultSize);

   /*
   // Set up a sizer with empty panel and a debug output area.
   wxFlexGridSizer *sizer = new wxFlexGridSizer (1, 2, 0, 0);
   main->SetSizer (sizer);
   sizer->AddGrowableCol (0, 5);
   sizer->AddGrowableCol (1, 1);
   sizer->AddGrowableRow (0, 0);
   sizer->SetFlexibleDirection (wxBOTH);
   sizer->Add (grdDebug, 0, wxEXPAND, 0);
   sizer->Add (new wxPanel (main), 0, wxEXPAND, 0);
   */
   
   if (m_isDebug)
   {
      // Do our test here.
      this->DoTest ();
   }
}

MainFrame::~MainFrame()
{
}

void MainFrame::DoTest ()
{
   #define BYTE_COUNT 500
   
   wxFileInputStream* stream;
   
   wxString  bits;
   uint      token;
   int       row;
   
   Console*  con;
   
   con = new Console ();
   
   stream = new wxFileInputStream (m_fileName);
   
   grdDebug->CreateGrid (0, 3, wxGrid::wxGridSelectCells);
   
   grdDebug->EnableDragRowSize (false);
   grdDebug->EnableEditing (false);
   
   grdDebug->SetColLabelValue (0, "Cnd");
   grdDebug->SetColLabelValue (1, "Instruction");
   
   for (row = 0; row < BYTE_COUNT; row++)
   {
      token = stream->GetC ();
      token = (token << 8) + stream->GetC ();
      token = (token << 8) + stream->GetC ();
      token = (token << 8) + stream->GetC ();
      
      con->DMA()->SetValue(row * 4, token);
      token = con->DMA()->GetValue(row * 4);
      
      bits = _T(UintToBitString (token));
      
      grdDebug->InsertRows (grdDebug->GetRows ());
      
      *(con->CPU ()->REG->PC ()->Value) = row * 4;
      
      con->CPU ()->DoSingleInstruction ();
      
      grdDebug->SetCellValue (row, 0, bits.Mid (0, 4));
      grdDebug->SetCellValue (row, 1, bits.Mid (4));
      grdDebug->SetCellValue (row, 2, con->CPU ()->LastResult);
      
      grdDebug->SetRowLabelValue (row, wxString::Format ("%d", row));
   }
   
   grdDebug->AutoSizeColumns ();
   
   delete con;
   delete stream;
}

void MainFrame::InitializeMenu ()
{
   wxMenuBar* mnuMain = new wxMenuBar ();
   wxMenu*    mnuFile = new wxMenu ();
   wxMenu*    mnuTools = new wxMenu ();
   wxMenu*    mnuHelp = new wxMenu ();
   
   this->SetMenuBar (mnuMain);
   
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
void MainFrame::OnMenuFileOpenISO (wxCommandEvent& WXUNUSED(event))
{
   wxString fileName = wxFileSelector (_T("Open 3DO ISO File"), NULL, NULL, NULL, _T("ISO Files (*.iso)|*.iso|All files (*.*)|*.*"), wxFD_OPEN | wxFD_FILE_MUST_EXIST);

   if (!fileName.empty())
   {
      wxMessageBox (wxString ("Nothing here yet. Try the browser."));
   }
}

void MainFrame::OnMenuFileExit (wxCommandEvent& WXUNUSED(event))
{
   this->Close ();
}

void MainFrame::OnMenuToolsBrowseISO (wxCommandEvent& WXUNUSED(event))
{
   this->BrowseIso ();
}

void MainFrame::OnMenuHelpAbout (wxCommandEvent& WXUNUSED(event))
{
   wxMessageBox (_T("FourDO - An Open-Source HLE 3DO Emulator\n\nVersion 0.0.0.1"), _T("About 4DO"), wxOK | wxICON_INFORMATION);
}

void MainFrame::BrowseIso ()
{
   wxString fileName = wxFileSelector (_T("Open 3DO ISO File"), NULL, NULL, NULL, _T("ISO Files (*.iso)|*.iso|All files (*.*)|*.*"), wxFD_OPEN | wxFD_FILE_MUST_EXIST);
   
   if (!fileName.empty())
   {
      this->BrowseIso (fileName);
   }
}

void MainFrame::BrowseIso (wxString fileName)
{
   ISOBrowser* browser;
   browser = new ISOBrowser (this, fileName);
   browser->Show();
   //ISOBrowser browser = ISOBrowser (this, fileName);
   //browser.Show();
}