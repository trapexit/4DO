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
   void onListRightClick(wxListEvent &event);
   void onListFocused(wxListEvent &event);

   void onPopupMenuOpenImage(wxCommandEvent &event);
   void onPopupMenuOpenCode(wxCommandEvent &event);
   void onPopupMenuOpenText(wxCommandEvent &event);

   void paintCurrentDirContents();

   wxTreeItemId findTreeItem(wxTreeItemId root, const wxString folder);
private:
   wxTreeCtrl*   tvwMain;
   wxTreeItemId  m_currentTreeRoot;
   wxListView*   lvwMain; 
   wxImageList*  imlIcons;
   wxString      m_fileName;

   // 
   // the filename of the list item that currently has
   // focuse in the iso browser
   // 
   wxString      m_focusedFile;

   Directory     *m_dir;

   DECLARE_EVENT_TABLE();
};

#endif // _INC_ISOBROWSE
