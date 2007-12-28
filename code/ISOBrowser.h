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
   
   void onListActivated(wxListEvent &event);
   void paintCurrentDirContents();

   wxTreeItemId findTreeItem(wxTreeItemId root, const wxString folder);
private:
   wxTreeCtrl*   tvwMain;
   wxTreeItemId  m_currentTreeRoot;
   wxListView*   lvwMain; 
   wxImageList*  imlIcons;
   wxString      m_fileName;

   wxString      m_currentPath;

   DECLARE_EVENT_TABLE();
};

#endif // _INC_ISOBROWSE
