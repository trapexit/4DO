#include "MainFrame.h"
#include "CodeViewer.h"
#include "ImageViewer.h"

#include "wx/settings.h"

#define  INSTRUCTIONS 11000

//Status bar
enum StatusBar
{
	SB_MENU,
	SB_INFO,
	SB_CNT
};

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
   EVT_MENU (ID_MENU_FILE_OPENISO,    MainFrame::OnMenuFileOpenISO)
   EVT_MENU (ID_MENU_FILE_OPENBINARY, MainFrame::OnMenuFileOpenBinary)
   EVT_MENU (ID_MENU_FILE_EXIT,       MainFrame::OnMenuFileExit)
   EVT_MENU (ID_MENU_TOOLS_BROWSEISO, MainFrame::OnMenuToolsBrowseISO)
   EVT_MENU (ID_MENU_TOOLS_VIEWCODE,  MainFrame::OnMenuToolsViewCode)
   EVT_MENU (ID_MENU_HELP_ABOUT,      MainFrame::OnMenuHelpAbout)
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
	// Create console
	m_con = new Console();
	
	/////////////////////
	// GUI Setup.
	this->SetTitle ("4DO");
	this->SetIcon (wxIcon(fourdo_xpm));
	this->SetSize (800, 600);
	this->CenterOnScreen ();
	
	this->SetBackgroundColour( wxSystemSettings::GetColour ( wxSYS_COLOUR_WINDOW ) );
	this->InitializeMenu ();
	
	wxBoxSizer* mainSizer = new wxBoxSizer (wxHORIZONTAL);
	ctlCanvas = new MainCanvas (this, wxID_ANY, m_con->DMA()->GetRAMPointer (0x002c0000));
	ctlCanvas->SetBackgroundColour (*wxLIGHT_GREY);
	mainSizer->Add (ctlCanvas, 1, wxEXPAND, 0, NULL);
	this->SetSizer(mainSizer);

	this->CreateStatusBar ();
	this->GetStatusBar()->SetFieldsCount( SB_CNT );
	this->SetStatusText( _T( "4DO: Open-Source HLE 3DO Emulator" ), SB_MENU );

	///////////////   
	if (m_isDebug)
	{
		// Do our test here.
		this->DoTest ();
	}
}

MainFrame::~MainFrame()
{
	delete m_con;
}

void MainFrame::DoTest ()
{
	#define ROM_LOAD_ADDRESS 0x03000000
	
	wxString 	bits;
	bool     	success;
	uint     	fileSize;
	wxStopWatch sw;
	
	/////////////////
	// Load the test
	if (m_imageFileName.Length () > 0)
	{
		// Open the Launchme of this CD image.
		uint32_t  bytesRead;
		File f(m_imageFileName);

		success = f.openFile("/launchme");
		if (!success)
		{
			// Error opening
			delete m_con;
			return;
		}
	
		// Load it into memory.
		fileSize = f.getFileSize ();
		f.read (m_con->DMA ()->GetRAMPointer (ROM_LOAD_ADDRESS), f.getFileSize (), &bytesRead);
		wxLogMessage (wxString::Format ("File size is %u", fileSize));
	}
	else if (m_codeFileName.Length () > 0)
	{
		// Open a code file.
		wxFile file;
		
		file.Open (m_codeFileName);
		file.Read (m_con->DMA()->GetRAMPointer (ROM_LOAD_ADDRESS), file.Length ());
		file.Close ();
	}
	
	//////////////////////////
	// Run program
	*(m_con->CPU ()->REG->PC ()->Value) = ROM_LOAD_ADDRESS;
	
	// Start timer.
	sw.Start ();
	
	// Run the program.
	for (uint row = 0; row < INSTRUCTIONS; row++)
	{
		// Process it.
		m_con->CPU ()->DoSingleInstruction ();
	}
	
	// End timer.
	sw.Pause ();
	
	// Display total time metric
	this->SetStatusText( wxString::Format( "Time: %ldms     Instructions:%u", sw.Time(), INSTRUCTIONS ), SB_INFO );
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
