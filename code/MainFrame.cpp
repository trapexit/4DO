#include "MainFrame.h"
#include "CodeViewer.h"

/////////////////////////////////////////////////////////////////////////
// Menu items.
/////////////////////////////////////////////////////////////////////////
enum Menu
{
   ID_MENU_FILE_OPENISO = 1,
   ID_MENU_FILE_EXIT,
   ID_MENU_TOOLS_BROWSEISO,
   ID_MENU_TOOLS_VIEWCODE,
   ID_MENU_HELP_ABOUT
};

BEGIN_EVENT_TABLE(MainFrame, wxFrame)
   EVT_MENU(ID_MENU_FILE_OPENISO, MainFrame::OnMenuFileOpenISO)
   EVT_MENU(ID_MENU_FILE_EXIT, MainFrame::OnMenuFileExit)
   EVT_MENU(ID_MENU_TOOLS_BROWSEISO, MainFrame::OnMenuToolsBrowseISO)
   EVT_MENU(ID_MENU_TOOLS_VIEWCODE, MainFrame::OnMenuToolsViewCode)
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
	
	//this->SetBackgroundStyle (wxBackgroundStyle::wxBG_STYLE_SYSTEM);
	//this->SetBackgroundColour (wxSystemColour::wxSYS_COLOUR_3DFACE);
	this->SetBackgroundColour (wxColor (0xFFFFFFFF));
	this->CreateStatusBar ();
	this->SetStatusText (_T("4DO: Open-Source HLE 3DO Emulator"));
   this->InitializeMenu ();	
	
   // Set up a sizer with empty panel and a debug output area.
   wxFlexGridSizer *sizerMain = new wxFlexGridSizer (1, 2, 0, 0);
   this->SetSizer (sizerMain);
   sizerMain->AddGrowableCol (0, 4);
   sizerMain->AddGrowableCol (1, 2);
   sizerMain->AddGrowableRow (0, 0);
   sizerMain->SetFlexibleDirection (wxBOTH);
	
	// A quick debug grid
	grdDebug = new wxGrid (this, wxID_ANY);
   sizerMain->Add (grdDebug, 0, wxEXPAND, 0);

   // CPU Status panel
	wxStaticBox *fraCPUStatus = new wxStaticBox(this, wxID_ANY, _T("&CPU Status"));
   wxSizer *sizerStatus = new wxStaticBoxSizer(fraCPUStatus, wxBOTH);
   sizerMain->Add (sizerStatus, 0, wxEXPAND, 0);
   fraCPUStatus->SetBackgroundColour (wxColor (0xFFFFFFFF));
   
   grdCPUStatus = new wxGrid (this, wxID_ANY);
   grdCPUStatus->CreateGrid (1, 2, wxGrid::wxGridSelectCells);
   grdCPUStatus->SetRowLabelSize (0);
   grdCPUStatus->SetColLabelSize (0);
   
   sizerStatus->Add (grdCPUStatus, 0, wxALL | wxGROW, 5);
	
   ///////////////   
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
   #define BYTE_COUNT 3000

   wxString  bits;
   Console*  con;
   bool      success;
   
   con = new Console ();
   
   /////////////////
   // Open Launchme!
	uint8_t   file[BYTE_COUNT];
	uint32_t  bytesRead;
	
	File f(m_fileName);

	success = f.openFile("/launchme");
	if (!success)
	{
		// Error opening
		delete con;
		return;
	}
   
   f.read(file, BYTE_COUNT, &bytesRead);
   
   /////////////////
   // Setup grid.
   grdDebug->CreateGrid (0, 3, wxGrid::wxGridSelectCells);
   grdDebug->EnableDragRowSize (false);
   grdDebug->EnableEditing (false);
   grdDebug->SetColLabelValue (0, "Cnd");
   grdDebug->SetColLabelValue (1, "Instruction");
   grdDebug->SetColLabelValue (2, "Last CPU Result");
   
   // Setup Status Grid
   grdCPUStatus->EnableDragRowSize (false);
   grdCPUStatus->EnableEditing (false);
   
   for (uint row = 0; row < BYTE_COUNT; row++)
   {
      // Read an instruction.
      uint token;
      token = (file[(row * 4)] << 24) + (file[(row * 4 + 1)] << 16) + (file[(row * 4 + 2)] << 8) + file[(row * 4 + 3)];
      bits = UintToBitString (token);

      // Write it memory temporarily...
      con->DMA ()->SetValue (row * 4, token);
      // Set PC there.
      *(con->CPU ()->REG->PC ()->Value) = row * 4;
      // Process it.
      con->CPU ()->DoSingleInstruction ();

      // Update CPU Status
      grdCPUStatus->DeleteRows (0, grdCPUStatus->GetRows ());
      grdCPUStatus->InsertRows (0, 37);

      int regNum = 0;
      grdCPUStatus->SetCellValue (regNum, 0, "R00");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R00))));
      grdCPUStatus->SetCellValue (regNum, 0, "R01");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R01))));
      grdCPUStatus->SetCellValue (regNum, 0, "R02");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R02))));
      grdCPUStatus->SetCellValue (regNum, 0, "R03");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R03))));
      grdCPUStatus->SetCellValue (regNum, 0, "R04");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R04))));
      grdCPUStatus->SetCellValue (regNum, 0, "R05");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R05))));
      grdCPUStatus->SetCellValue (regNum, 0, "R06");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R06))));
      grdCPUStatus->SetCellValue (regNum, 0, "R07");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R07))));
      grdCPUStatus->SetCellValue (regNum, 0, "R08");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R08))));
      grdCPUStatus->SetCellValue (regNum, 0, "R09");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R09))));
      grdCPUStatus->SetCellValue (regNum, 0, "R10");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R10))));
      grdCPUStatus->SetCellValue (regNum, 0, "R11");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R11))));
      grdCPUStatus->SetCellValue (regNum, 0, "R12");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R12))));
      grdCPUStatus->SetCellValue (regNum, 0, "R13");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R13))));
      grdCPUStatus->SetCellValue (regNum, 0, "R14");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R14))));
      grdCPUStatus->SetCellValue (regNum, 0, "PC");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_PC))));
      grdCPUStatus->SetCellValue (regNum, 0, "IR_R08_FIQ");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R08_FIQ))));
      grdCPUStatus->SetCellValue (regNum, 0, "IR_R09_FIQ");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R09_FIQ))));
      grdCPUStatus->SetCellValue (regNum, 0, "IR_R10_FIQ");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R10_FIQ))));
      grdCPUStatus->SetCellValue (regNum, 0, "IR_R11_FIQ");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R11_FIQ))));
      grdCPUStatus->SetCellValue (regNum, 0, "IR_R12_FIQ");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R12_FIQ))));
      grdCPUStatus->SetCellValue (regNum, 0, "IR_R13_FIQ");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R13_FIQ))));
      grdCPUStatus->SetCellValue (regNum, 0, "IR_R14_FIQ");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R14_FIQ))));
      grdCPUStatus->SetCellValue (regNum, 0, "IR_R13_SVC");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R13_SVC))));
      grdCPUStatus->SetCellValue (regNum, 0, "IR_R14_SVC");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R14_SVC))));
      grdCPUStatus->SetCellValue (regNum, 0, "IR_R13_ABT");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R13_ABT))));
      grdCPUStatus->SetCellValue (regNum, 0, "IR_R14_ABT");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R14_ABT))));
      grdCPUStatus->SetCellValue (regNum, 0, "IR_R13_IRQ");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R13_IRQ))));
      grdCPUStatus->SetCellValue (regNum, 0, "IR_R14_IRQ");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R14_IRQ))));
      grdCPUStatus->SetCellValue (regNum, 0, "IR_R13_UND");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R13_UND))));
      grdCPUStatus->SetCellValue (regNum, 0, "IR_R14_UND");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_R14_UND))));
      grdCPUStatus->SetCellValue (regNum, 0, "IR_CPSR");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_CPSR))));
      grdCPUStatus->SetCellValue (regNum, 0, "IR_SPSR_FIQ");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_SPSR_FIQ))));
      grdCPUStatus->SetCellValue (regNum, 0, "IR_SPSR_SVC");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_SPSR_SVC))));
      grdCPUStatus->SetCellValue (regNum, 0, "IR_SPSR_ABT");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_SPSR_ABT))));
      grdCPUStatus->SetCellValue (regNum, 0, "IR_SPSR_IRQ");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_SPSR_IRQ))));
      grdCPUStatus->SetCellValue (regNum, 0, "IR_SPSR_UND");
      grdCPUStatus->SetCellValue (regNum++, 1, wxString::Format ("%u", *(con->CPU ()->REG->Reg (IR_SPSR_UND))));
      
      //////////////
      // Make a new row.
      wxString cond;
      
      cond = wxString::Format ("%s (%s)",  bits.Mid (0, 4), con->CPU ()->LastCond);
      grdDebug->InsertRows (grdDebug->GetRows ());
      grdDebug->SetCellValue (row, 0, cond);
      grdDebug->SetCellValue (row, 1, bits.Mid (4));
      grdDebug->SetCellValue (row, 2, con->CPU ()->LastResult);
      grdDebug->SetRowLabelValue (row, wxString::Format ("%d", row));
   }
   
   /////////////////////
   // Auto size columns.
   grdDebug->AutoSizeColumns ();
   //grdDebug->AutoSizeRowLabelSize (0);
   
   delete con;
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
   mnuTools->Append (ID_MENU_TOOLS_VIEWCODE, _T("&View ARM60 Code"));
   
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

void MainFrame::OnMenuToolsViewCode (wxCommandEvent &WXUNUSED(event))
{
   wxString fileName = wxFileSelector (_T("Open ARM60 file"), NULL, NULL, NULL, _T("All files (*.*)|*.*"), wxFD_OPEN | wxFD_FILE_MUST_EXIST);
   
   if (!fileName.empty())
   {
      CodeViewer *codeViewer = new CodeViewer(this, fileName);
	  codeViewer->Show();
   }
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
}
