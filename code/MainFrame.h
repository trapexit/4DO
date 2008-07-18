#ifndef _INC_MAINFRAME
#define _INC_MAINFRAME

#include <iostream>

#include "wx/wx.h"
#include "wx/wfstream.h"
#include "wx/grid.h"
#include "wx/cmdline.h"

#include "ARM60CPU.h"
#include "types.h"
#include "Console.h"
#include "ISOBrowser.h"
#include "filesystem\file.h"

#include "fourdo.xpm" // Include the application's icon

class MainFrame : public wxFrame
{
public:
    MainFrame(wxCmdLineParser* parser);
    ~MainFrame();

   void OnMenuFileOpenISO    (wxCommandEvent &event);
   void OnMenuFileOpenBinary (wxCommandEvent &event);
   void OnMenuFileExit       (wxCommandEvent &event);
   void OnMenuToolsBrowseISO (wxCommandEvent &event);
   void OnMenuToolsViewCode  (wxCommandEvent &event);
   void OnMenuToolsTestVram  (wxCommandEvent &event);
   void OnMenuHelpAbout      (wxCommandEvent &event);

private:
   void InitializeMenu ();
   void DoTest ();
   bool ParseCommandLineArgs ();
   
   void UpdateGridRow (Console* con, int row, wxString caption, InternalRegisterType reg);

   void BrowseIso ();
   void BrowseIso (wxString fileName);

   wxGrid*   grdDebug;
   wxGrid*   grdCPUStatus;
   wxButton* btnNext;
   
   bool     m_isDebug;
   bool     m_loadFile;
   wxString m_imageFileName;
   wxString m_codeFileName;
   
   DECLARE_EVENT_TABLE ()
};

#endif //_INC_MAINFRAME
