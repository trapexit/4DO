#include "Main.h"
#include <iostream>

#include "types.h"
#include "Console.h"

#include "filesystem\file.h"

IMPLEMENT_APP(FourDOApp)

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
   // Parse command line arguments.
   if (!this->ParseCommandLineArgs ())
   {
      return false;
   }
   
   // Set up main window.
	wxFrame* main = new wxFrame ((wxFrame*) NULL, -1, _T("4DO"));
   main->SetIcon (wxIcon(kill_icon_xpm));
	main->SetSize (640, 480);
	main->CenterOnScreen ();
	main->SetBackgroundColour (wxColor (0xFF000000));
	main->CreateStatusBar ();
	main->SetStatusText (_T("4DO: Open-Source HLE 3DO Emulator"));
   InitializeMenu (main);	
	
	// A quick debug textbox.
	//txtBox = new wxTextCtrl (main, -1, wxEmptyString, wxDefaultPosition, wxDefaultSize, wxTE_MULTILINE);
	grdDebug = new wxGrid (main, -1, wxDefaultPosition, wxDefaultSize);

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
   
   // Show the form.
	main->Show (TRUE);
	SetTopWindow (main);
	
	return true;
}

bool FourDOApp::ParseCommandLineArgs ()
{
   int       n;
   bool      showUsage = false;
   bool      isDebug = false;
   bool      loadImage = false;
   wxString  fileName;
   wxString  arg;
   
   for (n = 1; n < this->argc; n++)
   {
      arg = (argv [n]);
      arg = arg.Trim ().MakeUpper ();
      if (arg.length() > 1 && (arg.StartsWith ("/") || arg.StartsWith ("-")))
      {
         // Correct format
         arg = arg.Mid (1);
         
         if (arg.Cmp ("D") == 0 || arg.Cmp ("DEBUG") == 0)
         {
            // Using debug mode.
            isDebug = true;
         }
         else if (arg.Cmp ("LI") == 0 || arg.Cmp ("LOADIMAGE") == 0)
         {
            if (n + 1 == this->argc)
            {
               // This was the last argument! There's no file to load.
               showUsage = true;
            }
            else
            {
               n++;
               fileName = this->argv [n];
            }
         }
      }
      else
      {
         // Incorrect format.
         showUsage = true;
      }
   }
   
   // If we're supposed to load an image. See if the image file exists first.
   if (loadImage)
   {
      if (!wxFileExists (fileName))
      {
         showUsage = true;
      }
   }
   
   // Now return failure/success.
   if (showUsage)
   {
      // Something went wrong.
      // TODO: Output usage summary?
      return false;
   }
   else
   {
      // We don't need to display the usage summary.
      m_isDebug = isDebug;
      m_loadFile = loadImage;
      m_fileName = fileName;
      return true;
   }
   
   // TODO: Investigate use of wxCmdLineParser... I found out about it after I coded this.
   /*
   static const wxCmdLineEntryDesc cmdLineDesc[] =
   {
       { wxCMD_LINE_SWITCH, _T("d"),  _T("debug"),     _T("Enable debug information") },
       { wxCMD_LINE_OPTION, _T("li"), _T("loadimage"), _T("Image file to load")       },
       { wxCMD_LINE_PARAM,  NULL, NULL, "input file", wxCMD_LINE_VAL_STRING, wxCMD_LINE_PARAM_MULTIPLE },
       { wxCMD_LINE_NONE }
   };
   
   //this->
   wxCmdLineParser* parser = new wxCmdLineParser ();
   parser->SetDesc (cmdLineDesc);
   parser->SetCmdLine (this->argc, this->argv);
   
   return true;
   */
   
}

void FourDOApp::DoTest ()
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
   bool ret;
   wxString fileName = wxFileSelector (_T("Open 3DO ISO File"), NULL, NULL, NULL, _T("ISO Files (*.iso)|*.iso|All files (*.*)|*.*"), wxFD_OPEN | wxFD_FILE_MUST_EXIST);

   if (!fileName.empty())
   {
       File f(fileName.c_str());

	   ret = f.openFile("/AppStartup");
	   
	   if (!ret)
	   {
	       wxMessageBox(_T("Error opening AppStartup"));
		   return;
	   }

       uint32_t bufLength = f.getFileSize(), bytesRead;
       uint8_t  *buf = new uint8_t[bufLength];

       ret = f.read(buf, bufLength, &bytesRead);

       if (!ret)
       {
           printf("read failed\n");

           if (buf)
               delete[] buf;

           return;
       }

	   wxString fileContents(buf, bytesRead);

	   wxMessageBox(fileContents);

       if (buf)
           delete[] buf;
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
