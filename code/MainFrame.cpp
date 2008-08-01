#include "MainFrame.h"
#include "ImageViewer.h"

#include "wx/settings.h"

#define  REFRESH_DELAY    50
#define  ROM_LOAD_ADDRESS 0x03000000

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
   ID_MENU_FILE_OPENCODE,
   ID_MENU_FILE_EXIT,
   ID_MENU_CONSOLE_RESET,
   ID_MENU_TOOLS_BROWSEISO,
   ID_MENU_HELP_ABOUT
};

enum Timer
{
	ID_TIMER_MAIN = 1,
};

BEGIN_EVENT_TABLE( MainFrame, wxFrame )
   EVT_MENU ( ID_MENU_FILE_OPENISO,    MainFrame::OnMenuFileOpenISO )
   EVT_MENU ( ID_MENU_FILE_OPENCODE,   MainFrame::OnMenuFileOpenCode)
   EVT_MENU ( ID_MENU_FILE_EXIT,       MainFrame::OnMenuFileExit )
   EVT_MENU ( ID_MENU_CONSOLE_RESET,   MainFrame::OnMenuConsoleReset )
   EVT_MENU ( ID_MENU_TOOLS_BROWSEISO, MainFrame::OnMenuToolsBrowseISO )
   EVT_MENU ( ID_MENU_HELP_ABOUT,      MainFrame::OnMenuHelpAbout )
   EVT_TIMER( ID_TIMER_MAIN,	       MainFrame::OnMainTimer )
END_EVENT_TABLE()

/////////////////////////////////////////////////////////////////////////
// Frame startup
/////////////////////////////////////////////////////////////////////////

MainFrame::MainFrame( wxCmdLineParser* parser )
		: wxFrame( ( wxFrame * ) NULL, -1, wxEmptyString, wxDefaultPosition, wxDefaultSize )
{
	wxString fileName;

	// Create a console
	m_con = new Console();
	m_con->CPU()->SetPC( ROM_LOAD_ADDRESS );

	/////////////////////
	// GUI Setup.
	this->SetTitle( "4DO" );
	this->SetIcon( wxIcon( fourdo_xpm ) );
	this->SetSize( 648, 546 );
	this->CenterOnScreen();
	
	this->SetBackgroundColour( wxSystemSettings::GetColour ( wxSYS_COLOUR_WINDOW ) );
	this->InitializeMenu ();
	
	wxBoxSizer* mainSizer = new wxBoxSizer( wxHORIZONTAL );
	ctlCanvas = new MainCanvas( this, wxID_ANY, m_con->DMA()->GetRAMPointer( 0x002c0000 ) );
	ctlCanvas->SetBackgroundColour( *wxLIGHT_GREY );
	mainSizer->Add( ctlCanvas, 1, wxEXPAND, 0, NULL );
	this->SetSizer( mainSizer );

	this->CreateStatusBar();
	this->GetStatusBar()->SetFieldsCount( SB_CNT );
	this->SetStatusText( _T( "4DO: Open-Source HLE 3DO Emulator" ), SB_MENU );

	///////////////
	// Set up timer?
	tmrMain = new wxTimer( this, ID_TIMER_MAIN );
	tmrMain->Start( REFRESH_DELAY );

	/////////////////////
	// Handle command-line arguments.
	if( parser->Found( "li" ) )
	{
		parser->Found( "li", &fileName );
		this->ConsoleLoadIso( fileName );
	}
	else if( parser->Found( "lc" ) )
	{
		parser->Found( "lc", &fileName );
		this->ConsoleLoadCode( fileName );
	}
}

MainFrame::~MainFrame()
{
	delete m_con;
}

void MainFrame::RunCycles() 
{
	if( m_consoleRunning )
	{
		uint	    cycles;
		wxStopWatch sw;
		
		//////////////////////////////
		// Start execution timer.
		sw.Start();
		
		//cycles = time_used / 0.00008;
		cycles = REFRESH_DELAY / 0.00008;
		m_con->CPU()->Execute( cycles );

		// End execution timer.
		sw.Pause();
		
		// Display total time metric
		this->SetStatusText( wxString::Format( "Time: %ldms     Cycles:%u", sw.Time(), cycles ), SB_INFO );
	}
	else
	{
		// Display total time metric
		this->SetStatusText( _T( "Not Running" ), SB_INFO );
	}
}

void MainFrame::InitializeMenu ()
{
	wxMenuBar* mnuMain    = new wxMenuBar ();
	wxMenu*    mnuFile    = new wxMenu ();
	wxMenu*    mnuTools   = new wxMenu ();
	wxMenu*    mnuHelp    = new wxMenu ();

	this->SetMenuBar (mnuMain);

	//////////////////////
	// File menu
	mnuMain->Append( mnuFile, _T( "&File" ));
	mnuFile->Append( ID_MENU_FILE_OPENISO,  _T( "&Open ISO...\tCtrl+O" ) );
	mnuFile->Append( ID_MENU_FILE_OPENCODE, _T( "&Open ARM Binary..." ) );
	mnuFile->AppendSeparator();
	mnuFile->Append( ID_MENU_FILE_EXIT, _T( "&Exit\tCtrl+X" ) );

	/*
	//////////////////////
	// Console menu
	mnuMain->Append( mnuConsole, _T( "&Console" ) );
	mnuConsole->Append( ID_MENU_CONSOLE_RESET, _T( "&Reset...\tCtrl+R" ) );
	*/

	//////////////////////
	// Tools menu
	mnuMain->Append( mnuTools, _T( "&Tools" ) );
	mnuTools->Append( ID_MENU_TOOLS_BROWSEISO, _T( "&Browse ISO...\tCtrl+B" ) );

	//////////////////////
	// Help menu
	mnuMain->Append( mnuHelp, _T( "&Help" ) );
	mnuHelp->Append( ID_MENU_HELP_ABOUT, _T( "&About...\tShift+F1" ) );
}

/////////////////////////////////////////////////////////////////////////
// Event handlers
/////////////////////////////////////////////////////////////////////////
void MainFrame::OnMainTimer( wxTimerEvent &event )
{
	this->RunCycles();
	ctlCanvas->UpdateImage();
	ctlCanvas->Refresh();
}

void MainFrame::OnMenuFileOpenISO( wxCommandEvent& WXUNUSED( event ) )
{
	wxString fileName = wxFileSelector( _T( "Open 3DO ISO File" ), NULL, NULL, NULL, _T( "ISO Files (*.iso)|*.iso|All files (*.*)|*.*" ), wxFD_OPEN | wxFD_FILE_MUST_EXIST );

	if( !fileName.empty() )
	{
		this->ConsoleLoadIso( fileName );
	}
}

void MainFrame::OnMenuFileOpenCode( wxCommandEvent& WXUNUSED( event ) )
{
	wxString fileName = wxFileSelector( _T( "Open ARM Code File" ), NULL, NULL, NULL, _T( "Code files (*.3bn)|*.3bn|All files (*.*)|*.*" ), wxFD_OPEN | wxFD_FILE_MUST_EXIST );

	if( !fileName.empty() )
	{
		this->ConsoleLoadCode( fileName );
	}
}

void MainFrame::OnMenuFileExit (wxCommandEvent& WXUNUSED( event ) )
{
	this->Close();
}

void MainFrame::OnMenuToolsBrowseISO( wxCommandEvent& WXUNUSED( event ) )
{
	this->BrowseIso();
}

void MainFrame::OnMenuHelpAbout( wxCommandEvent& WXUNUSED( event ) )
{
	wxMessageBox( _T( "FourDO - An Open-Source HLE 3DO Emulator\n\nVersion 0.0.0.1" ), _T( "About 4DO" ), wxOK | wxICON_INFORMATION );
}

void MainFrame::OnMenuConsoleReset( wxCommandEvent& WXUNUSED( event ) )
{
	this->ConsoleReset();
}

void MainFrame::BrowseIso()
{
	wxString fileName = wxFileSelector( _T( "Open 3DO ISO File" ), NULL, NULL, NULL, _T( "ISO Files (*.iso)|*.iso|All files (*.*)|*.*" ), wxFD_OPEN | wxFD_FILE_MUST_EXIST );
	
	if( !fileName.empty() )
	{
		this->BrowseIso( fileName );
	}
}

void MainFrame::BrowseIso( wxString fileName )
{
	ISOBrowser* browser;
	browser = new ISOBrowser( this, fileName );
	browser->Show();
}

void MainFrame::ConsoleReset ()
{
	/////////////////////
	// TODO: Properly reset console
	
	// Initialize the PC
	m_con->CPU()->Reset();
	
	m_loadIso = false;
	m_fileName = wxEmptyString;
	m_consoleRunning = false;
}

void MainFrame::ConsoleLoadIso( wxString fileName )
{
	bool  success;
	uint  fileSize;
	
	/////////////////
	// Load the ISO

	// Open the Launchme of this CD image.
	uint32_t  bytesRead;
	File f( fileName );

	success = f.openFile( "/launchme" );
	if( !success )
	{
		// Error opening
		return;
	}

	// Load it into memory.
	fileSize = f.getFileSize();
	f.read( m_con->DMA ()->GetRAMPointer( ROM_LOAD_ADDRESS ), f.getFileSize (), &bytesRead );
	
	// Initialize the PC
	m_con->CPU()->SetPC( ROM_LOAD_ADDRESS );

	m_loadIso = true;
	m_fileName = fileName;
	m_consoleRunning = true;
}

void MainFrame::ConsoleLoadCode ( wxString fileName )
{
	// Open a code file.
	wxFile file;
	
	file.Open( fileName );
	file.Read( m_con->DMA()->GetRAMPointer( ROM_LOAD_ADDRESS ), file.Length () );
	file.Close();

	// Initialize the PC
	m_con->CPU()->SetPC( ROM_LOAD_ADDRESS );
	
	m_loadIso = false;
	m_fileName = fileName;
	m_consoleRunning = true;
}