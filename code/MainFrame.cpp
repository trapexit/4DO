#include "MainFrame.h"
#include "CodeViewer.h"
#include "ImageViewer.h"

/////////////////////////////////////////////////////////////////////////
// Menu items.
/////////////////////////////////////////////////////////////////////////
enum Menu
{
   ID_MENU_FILE_OPENISO = 1,
   ID_MENU_FILE_OPENBINARY,
   ID_MENU_FILE_EXIT,
   ID_MENU_TOOLS_BROWSEISO,
   ID_MENU_TOOLS_VIEWCODE,
   ID_MENU_TOOLS_TESTVRAM,
   ID_MENU_HELP_ABOUT
};

BEGIN_EVENT_TABLE(MainFrame, wxFrame)
   EVT_MENU(ID_MENU_FILE_OPENISO, MainFrame::OnMenuFileOpenISO)
   EVT_MENU(ID_MENU_FILE_OPENBINARY, MainFrame::OnMenuFileOpenBinary)
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
		parser->Found ("li", &m_imageFileName);
	}
	if (parser->Found ("lc"))
	{
		parser->Found ("lc", &m_codeFileName);
	}
	
	/////////////////////
	// GUI Setup.
	this->SetTitle ("4DO");
	this->SetIcon (wxIcon(fourdo_xpm));
	this->SetSize (1100, 800);
	this->CenterOnScreen ();
	
	this->SetBackgroundColour (wxColor (0xFFFFFFFF));
	this->CreateStatusBar ();
	this->SetStatusText (_T("4DO: Open-Source HLE 3DO Emulator"));
	this->InitializeMenu ();
	
	wxBoxSizer * mainSizer = new wxBoxSizer (wxHORIZONTAL);
	wxBoxSizer * leftSizer = new wxBoxSizer (wxVERTICAL);
	wxBoxSizer * buttonSizer = new wxBoxSizer (wxHORIZONTAL);
	
	// Process the next message button.
	btnNext = new wxButton (this, wxID_ANY, _T("&Next"), wxDefaultPosition, wxDefaultSize, 0, wxDefaultValidator, wxButtonNameStr);
	buttonSizer->Add (btnNext, 0, wxALIGN_RIGHT | wxALL, 4, NULL);
	
	// A quick debug grid
	grdDebug = new wxGrid (this, wxID_ANY);
	mainSizer->Add (leftSizer, 1, wxEXPAND, 4, NULL);
	
	leftSizer->Add (buttonSizer, 0, wxEXPAND | wxALL, 0, NULL);
	leftSizer->Add (grdDebug, 1, wxEXPAND | wxALL, 4, NULL);

	// CPU Status panel
	wxStaticBox *fraCPUStatus = new wxStaticBox(this, wxID_ANY, _T("&CPU Status"));
	wxSizer *sizerStatus = new wxStaticBoxSizer(fraCPUStatus, wxVERTICAL);
	mainSizer->Add (sizerStatus, 1, wxEXPAND | wxALL, 4, NULL);
	fraCPUStatus->SetBackgroundColour (wxColor (0xFFFFFFFF));
	
	grdCPUStatus = new wxGrid (this, wxID_ANY);
	grdCPUStatus->CreateGrid (1, 4, wxGrid::wxGridSelectCells);
	grdCPUStatus->SetRowLabelSize (0);
	grdCPUStatus->SetColLabelSize (20);
	
	sizerStatus->Add (grdCPUStatus, 1, wxEXPAND | wxALL , 4);
	
	this->SetSizer (mainSizer);
	
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
	#define ROM_LOAD_ADDRESS 0x03000000
	#define INSTRUCTIONS     11000

	wxString 	bits;
	Console* 	con;
	bool     	success;
	uint     	fileSize;
	uint	 	PCBefore;
	wxStopWatch sw;
	
	con = new Console ();
	
	/////////////////
	
	if (m_imageFileName.Length () > 0)
	{
		// Open the Launchme of this CD image.
		uint32_t  bytesRead;
		File f(m_imageFileName);

		success = f.openFile("/launchme");
		if (!success)
		{
			// Error opening
			delete con;
			return;
		}
	
		// Load it into memory.
		fileSize = f.getFileSize ();
		f.read (con->DMA ()->GetRAMPointer (ROM_LOAD_ADDRESS), f.getFileSize (), &bytesRead);
		wxLogMessage (wxString::Format ("File size is %u", fileSize));
	}
	else if (m_codeFileName.Length () > 0)
	{
		// Open a code file.
		wxFile file;
		
		file.Open (m_codeFileName);
		file.Read (con->DMA()->GetRAMPointer (ROM_LOAD_ADDRESS), file.Length ());
		file.Close ();
	}
	
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
	
	grdCPUStatus->SetColLabelValue (0, "Reg");
	grdCPUStatus->SetColLabelValue (1, "Val Hex[Neg]");
	grdCPUStatus->SetColLabelValue (2, "Val Dec[Neg]");
	grdCPUStatus->SetColLabelValue (3, "Val Bin");
	
	*(con->CPU ()->REG->PC ()->Value) = ROM_LOAD_ADDRESS;
	
	/*
	for (uint row = 0; row < INSTRUCTIONS; row++)
	{
		// Read an instruction.
		uint token;
		
		PCBefore = *(con->CPU ()->REG->PC ()->Value);
		token = con->DMA ()->GetWord (PCBefore);

		// Convert this thing to bits.
		bits = UintToBitString (token);
		
		// Process it.
		con->CPU ()->DoSingleInstruction ();

		// Update CPU Status
		grdCPUStatus->DeleteRows (0, grdCPUStatus->GetRows ());
		grdCPUStatus->InsertRows (0, 37);

		int regNum = 0;
		this->UpdateGridRow (con, regNum++, "R00", IR_R00);
		this->UpdateGridRow (con, regNum++, "R01", IR_R01);
		this->UpdateGridRow (con, regNum++, "R02", IR_R02);
		this->UpdateGridRow (con, regNum++, "R03", IR_R03);
		this->UpdateGridRow (con, regNum++, "R04", IR_R04);
		this->UpdateGridRow (con, regNum++, "R05", IR_R05);
		this->UpdateGridRow (con, regNum++, "R06", IR_R06);
		this->UpdateGridRow (con, regNum++, "R07", IR_R07);
		this->UpdateGridRow (con, regNum++, "R08", IR_R08);
		this->UpdateGridRow (con, regNum++, "R09", IR_R09);
		this->UpdateGridRow (con, regNum++, "R10", IR_R10);
		this->UpdateGridRow (con, regNum++, "R11", IR_R11);
		this->UpdateGridRow (con, regNum++, "R12", IR_R12);
		this->UpdateGridRow (con, regNum++, "R13", IR_R13);
		this->UpdateGridRow (con, regNum++, "R14", IR_R14);
		this->UpdateGridRow (con, regNum++, "PC", IR_PC);
		this->UpdateGridRow (con, regNum++, "R08_FIQ", IR_R08_FIQ);
		this->UpdateGridRow (con, regNum++, "R09_FIQ", IR_R09_FIQ);
		this->UpdateGridRow (con, regNum++, "R10_FIQ", IR_R10_FIQ);
		this->UpdateGridRow (con, regNum++, "R11_FIQ", IR_R11_FIQ);
		this->UpdateGridRow (con, regNum++, "R12_FIQ", IR_R12_FIQ);
		this->UpdateGridRow (con, regNum++, "R13_FIQ", IR_R13_FIQ);
		this->UpdateGridRow (con, regNum++, "R14_FIQ", IR_R14_FIQ);
		this->UpdateGridRow (con, regNum++, "R13_SVC", IR_R13_SVC);
		this->UpdateGridRow (con, regNum++, "R14_SVC", IR_R14_SVC);
		this->UpdateGridRow (con, regNum++, "R13_ABT", IR_R13_ABT);
		this->UpdateGridRow (con, regNum++, "R14_ABT", IR_R14_ABT);
		this->UpdateGridRow (con, regNum++, "R13_IRQ", IR_R13_IRQ);
		this->UpdateGridRow (con, regNum++, "R14_IRQ", IR_R14_IRQ);
		this->UpdateGridRow (con, regNum++, "R13_UND", IR_R13_UND);
		this->UpdateGridRow (con, regNum++, "R14_UND", IR_R14_UND);
		this->UpdateGridRow (con, regNum++, "CPSR", IR_CPSR);
		this->UpdateGridRow (con, regNum++, "SPSR_FIQ", IR_SPSR_FIQ);
		this->UpdateGridRow (con, regNum++, "SPSR_SVC", IR_SPSR_SVC);
		this->UpdateGridRow (con, regNum++, "SPSR_ABT", IR_SPSR_ABT);
		this->UpdateGridRow (con, regNum++, "SPSR_IRQ", IR_SPSR_IRQ);
		this->UpdateGridRow (con, regNum++, "SPSR_UND", IR_SPSR_UND);
		
		//////////////
		// Make a new row.
		wxString cond;
		
		cond = wxString::Format ("%s (%s)",  bits.Mid (0, 4), con->CPU ()->LastCond);
		grdDebug->InsertRows (grdDebug->GetRows ());
		grdDebug->SetCellValue (row, 0, cond);
		grdDebug->SetCellValue (row, 1, bits.Mid (4));
		grdDebug->SetCellValue (row, 2, con->CPU ()->LastResult);
		grdDebug->SetRowLabelValue (row, UintToHexString (PCBefore));
	}
	
	*/
	
	// Start timer.
	sw.Start ();
	
	// Run the program.
	for (uint row = 0; row < INSTRUCTIONS; row++)
	{
		// Process it.
		con->CPU ()->DoSingleInstruction ();
	}
	
	// End timer.
	sw.Pause ();
	
	// Display total time metric
	grdDebug->InsertRows( grdDebug->GetRows () );
	grdDebug->SetCellValue
		( 
		grdDebug->GetRows () - 1,  
		0, 
		wxString::Format("Time: %ldms", sw.Time() ) 
		);
	
	/////////////////////
	// Auto size columns.
	grdDebug->AutoSizeColumns ();
	grdCPUStatus->AutoSizeColumns ();
	//grdDebug->AutoSizeRowLabelSize (0);
	
	ImageViewer* imageViewer;
	imageViewer = new ImageViewer(this, con->DMA()->GetRAMPointer (0x002c0000));
	imageViewer->Show();		
	
	delete con;
}

void MainFrame::UpdateGridRow (Console* con, int row, wxString caption, InternalRegisterType reg)
{
	uint      regValue; 
	wxString  bits;
	wxString  hex;
	wxString  hexNeg;
	
	// Get the value of the register.
	regValue = *(con->CPU ()->REG->Reg (reg));
	
	// Turn it into a bit string.
	bits = UintToBitString (regValue);
	hex = UintToHexString (regValue);
	hexNeg = UintToHexString ((~regValue)+1);
	
	grdCPUStatus->SetCellValue (row, 0, caption);
	grdCPUStatus->SetCellTextColour (row, 0, wxColour (128, 128, 128));
	grdCPUStatus->SetCellValue (row, 1, wxString::Format ("%s [%s]", hex, hexNeg));
	grdCPUStatus->SetCellValue (row, 2, wxString::Format ("%u [%i]", regValue, regValue));
	grdCPUStatus->SetCellValue (row, 3, bits);
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
	mnuFile->Append (ID_MENU_FILE_OPENISO,    _T("&Open ISO...\tCtrl+O"));
	mnuFile->Append (ID_MENU_FILE_OPENBINARY, _T("&Open ARM Binary..."));
	mnuFile->AppendSeparator ();
	mnuFile->Append (ID_MENU_FILE_EXIT, _T("&Exit\tCtrl+X"));

   //////////////////////
   // Tools menu
   mnuMain->Append (mnuTools, _T("&Tools"));
   mnuTools->Append (ID_MENU_TOOLS_BROWSEISO, _T("&Browse ISO...\tCtrl+B"));
   mnuTools->Append (ID_MENU_TOOLS_VIEWCODE, _T("&View ARM60 Code"));
   mnuTools->Append (ID_MENU_TOOLS_TESTVRAM, _T("&Test VRAM"));
   
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

void MainFrame::OnMenuFileOpenBinary (wxCommandEvent& WXUNUSED(event))
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
