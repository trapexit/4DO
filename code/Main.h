#include "wx/wx.h"
#include "wx/wfstream.h"
#include "wx/grid.h"
//#include "wx/cmdline.h"
//#include "wx/process.h"

#include "4DO.xpm" // Include the application's icon

class FourDOApp : public wxApp
{
public:
   virtual bool OnInit();
   void OnMenuFileOpenISO (wxCommandEvent& event);
   void OnMenuFileExit (wxCommandEvent& event);
   void OnMenuToolsBrowseISO (wxCommandEvent& event);
   void OnMenuHelpAbout (wxCommandEvent& event);

private:
   void InitializeMenu (wxFrame* frame);
   void DoTest ();
   bool ParseCommandLineArgs ();

   //wxTextCtrl* txtBox;
   wxGrid*     grdDebug;
   
   bool     m_isDebug;
   bool     m_loadFile;
   wxString m_fileName;
     
DECLARE_EVENT_TABLE ()
};

DECLARE_APP(FourDOApp)