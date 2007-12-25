#ifndef _INC_ISOBROWSE
#define _INC_ISOBROWSE

#include "wx/wx.h"
#include "wx/treectrl.h"
#include "wx/imaglist.h"
#include "wx/listctrl.h"

#include "folder.xpm"
#include "file.xpm"
#include "filesystem\file.h"

class ISOBrowser : public wxFrame
{
public:
   ISOBrowser(wxFrame* parent, wxString fileName);
   ~ISOBrowser();
   
private:
   wxTreeCtrl*   tvwMain;
   wxListView*   lvwMain; 
   wxImageList*  imlIcons;
   wxString      m_fileName;
};

#endif // _INC_ISOBROWSE