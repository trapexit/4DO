#include "ISOBrowser.h"

/////////////////////////////////////////////////////////////////////////
// Event declaration
/////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////
// Frame startup
/////////////////////////////////////////////////////////////////////////
ISOBrowser::ISOBrowser(wxFrame* parent, wxString fileName)
      : wxFrame (parent, -1, wxEmptyString, wxDefaultPosition, wxDefaultSize)
{
   this->SetTitle (wxString().Format ("ISO Browser - %s", fileName));
   
   /////////////////////////
   
   Directory       dir (fileName);
   DirectoryEntry  de;
   int             image;

   wxTreeItemId objRoot;
   m_fileName = fileName;
   
   imlIcons = new wxImageList (16, 16);
   imlIcons->Add (wxIcon (folder_xpm));
   imlIcons->Add (wxIcon (file_xpm));
   
   tvwMain = new wxTreeCtrl (this, -1, wxDefaultPosition, wxDefaultSize, wxTR_HIDE_ROOT);
   tvwMain->SetImageList (imlIcons);
   objRoot = tvwMain->AddRoot (_T("My Root"), 0);

   lvwMain = new wxListView (this, -1);
   lvwMain->SetImageList (imlIcons, wxIMAGE_LIST_SMALL);
   lvwMain->InsertColumn (lvwMain->GetColumnCount(), _T("File Name"));
   lvwMain->InsertColumn (lvwMain->GetColumnCount(), _T("Block Size"));
   lvwMain->InsertColumn (lvwMain->GetColumnCount(), _T("Copies"));
   lvwMain->InsertColumn (lvwMain->GetColumnCount(), _T("Entry Length Blocks"));
   lvwMain->InsertColumn (lvwMain->GetColumnCount(), _T("Entry Length Bytes"));
   lvwMain->InsertColumn (lvwMain->GetColumnCount(), _T("Extension"));
   lvwMain->InsertColumn (lvwMain->GetColumnCount(), _T("Flags"));
   lvwMain->InsertColumn (lvwMain->GetColumnCount(), _T("Gap"));
   lvwMain->InsertColumn (lvwMain->GetColumnCount(), _T("Id"));
   lvwMain->InsertColumn (lvwMain->GetColumnCount(), _T("Last Copy"));
   
   dir.openDirectory ("/");
   while (dir.enumerateDirectory (&de))
   {
      if ((de.flags & DirectoryEntryTypeMask) == DirectoryEntryTypeFolder)
      {
         // Folder.
         image = 0;
         tvwMain->AppendItem (objRoot, wxString (de.fileName), image);
      }
      else
      {
         // File!
         image = 1;
      }
      
      // Always append to the listview.
      lvwMain->InsertItem (lvwMain->GetItemCount (), wxString (de.fileName), image);
      lvwMain->SetItem (lvwMain->GetItemCount () - 1, 1, wxString::Format ("%d", de.blockSize));
      lvwMain->SetItem (lvwMain->GetItemCount () - 1, 2, wxString::Format ("%d", de.copies));
      lvwMain->SetItem (lvwMain->GetItemCount () - 1, 3, wxString::Format ("%d", de.entryLengthBlocks));
      lvwMain->SetItem (lvwMain->GetItemCount () - 1, 4, wxString::Format ("%d", de.entryLengthBytes));
      lvwMain->SetItem (lvwMain->GetItemCount () - 1, 5, wxString (de.ext));
      lvwMain->SetItem (lvwMain->GetItemCount () - 1, 6, wxString::Format ("%d", de.flags));
      lvwMain->SetItem (lvwMain->GetItemCount () - 1, 7, wxString::Format ("%d", de.gap));
      lvwMain->SetItem (lvwMain->GetItemCount () - 1, 8, wxString::Format ("%d", de.id));
      lvwMain->SetItem (lvwMain->GetItemCount () - 1, 9, wxString::Format ("%d", de.lastCopy));
   }
   for (int x = 0; x < lvwMain->GetColumnCount(); x++)
   {
      lvwMain->SetColumnWidth (x, wxLIST_AUTOSIZE_USEHEADER);
   }
   
   // Set up a sizer that splits the frame in two.
   wxFlexGridSizer *sizer = new wxFlexGridSizer (1, 2, 0, 0);
   this->SetSizer (sizer);
   sizer->AddGrowableCol (0, 1);
   sizer->AddGrowableCol (1, 2);
   sizer->AddGrowableRow (0, 0);
   sizer->SetFlexibleDirection (wxBOTH);
   sizer->Add (tvwMain, 0, wxEXPAND, 0);
   sizer->Add (lvwMain, 0, wxEXPAND, 0);

   // I'm doing this for the resize handle.
   this->SetStatusBar (new wxStatusBar (this));
   
	this->SetSize (512, 384);
	this->CenterOnScreen ();
}

ISOBrowser::~ISOBrowser()
{
   delete imlIcons;
}