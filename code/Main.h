#include "wx/wxprec.h"

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
     
DECLARE_EVENT_TABLE ()
};

DECLARE_APP(FourDOApp)