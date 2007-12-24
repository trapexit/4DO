#include "ISOBrowser.h"

ISOBrowser::ISOBrowser(wxFrame* parent, wxString fileName)
      : wxFrame (parent, -1, wxEmptyString, wxDefaultPosition, wxDefaultSize)
{
   Directory       dir (fileName);
   DirectoryEntry  de;

   wxTreeItemId objRoot;
   m_fileName = fileName;
   
   imlIcons = new wxImageList (16, 16);
   imlIcons->Add (wxIcon (folder_xpm));
   imlIcons->Add (wxIcon (plugin_xpm));
   
   tvwMain = new wxTreeCtrl (this, -1, wxDefaultPosition, wxDefaultSize, wxTR_HIDE_ROOT);
   tvwMain->SetImageList (imlIcons);
   
   objRoot = tvwMain->AddRoot (_T("My Root"), 0);
   dir.openDirectory ("/");
   while (dir.enumerateDirectory (&de))
   {
      if ((de.flags & DirectoryEntryTypeMask) == DirectoryEntryTypeFolder)
         tvwMain->AppendItem (objRoot, wxString (de.fileName), 0);
      else
         tvwMain->AppendItem (objRoot, wxString (de.fileName), 1);
   }
   
   /*
   // Set up a sizer that splits the frame in two.
   wxFlexGridSizer *sizer = new wxFlexGridSizer (1, 2, 0, 0);
   this->SetSizer (sizer);
   sizer->AddGrowableCol (0, 1);
   sizer->AddGrowableCol (1, 1);
   sizer->AddGrowableRow (0, 0);
   sizer->SetFlexibleDirection (wxBOTH);
   sizer->Add (tvwMain, 0, wxEXPAND, 0);
   sizer->Add (new wxPanel (this), 0, wxEXPAND, 0);
   */

   // I'm doing this for the resize handle.
   this->SetStatusBar (new wxStatusBar (this));
   
	this->SetSize (512, 384);
	this->CenterOnScreen ();
}

ISOBrowser::~ISOBrowser()
{
   delete imlIcons;
}  