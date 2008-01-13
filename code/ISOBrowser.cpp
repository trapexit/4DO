#include "ISOBrowser.h"
#include "ImageViewer.h"
#include "CodeViewer.h"

enum IDs
{
	ID_LIST_VIEW = 1,
	ID_TREE_VIEW,
	
	ID_MENU_OPEN_IMAGE,
	ID_MENU_OPEN_CODE,
	ID_MENU_OPEN_TEXT
};

enum imageIds
{
	IMAGE_ID_FOLDER = 0,
	IMAGE_ID_FILE
};

int wxCALLBACK listSortFunction(long item1, long item2, long WXUNUSED(data))
{
	if (item1 > item2)
		return 1;
	else if (item1 < item2)
		return -1;
	else
		return 0;
}

/////////////////////////////////////////////////////////////////////////
// Event declaration
/////////////////////////////////////////////////////////////////////////
BEGIN_EVENT_TABLE(ISOBrowser, wxFrame)
	EVT_LIST_ITEM_ACTIVATED(ID_LIST_VIEW, ISOBrowser::onListActivated)
	EVT_LIST_ITEM_RIGHT_CLICK(ID_LIST_VIEW, ISOBrowser::onListRightClick)
	EVT_LIST_ITEM_FOCUSED(ID_LIST_VIEW, ISOBrowser::onListFocused)
	EVT_MENU(ID_MENU_OPEN_IMAGE, ISOBrowser::onPopupMenuOpenImage)
	EVT_MENU(ID_MENU_OPEN_CODE, ISOBrowser::onPopupMenuOpenCode)
	EVT_MENU(ID_MENU_OPEN_TEXT, ISOBrowser::onPopupMenuOpenText)
END_EVENT_TABLE()

/////////////////////////////////////////////////////////////////////////
// Frame startup
/////////////////////////////////////////////////////////////////////////

ISOBrowser::ISOBrowser(wxFrame* parent, wxString fileName)
      : wxFrame (parent, -1, wxEmptyString, wxDefaultPosition, wxDefaultSize)
{
   this->SetIcon(folder_xpm);
   this->SetTitle (wxString().Format ("ISO Browser - %s", fileName));
   
   /////////////////////////
   
   m_fileName = fileName;
   
   // 
   // start out at the root directory
   // 
   m_dir = new Directory(m_fileName);
   m_dir->openDirectory("/");

   imlIcons = new wxImageList (16, 16);
   imlIcons->Add (wxIcon (folder_xpm));
   imlIcons->Add (wxIcon (file_xpm));
   
   tvwMain = new wxTreeCtrl (this, ID_TREE_VIEW);
   tvwMain->SetImageList (imlIcons);
   m_currentTreeRoot = tvwMain->AddRoot (_T(m_dir->getPath()), 0);

   lvwMain = new wxListView (this, ID_LIST_VIEW);
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
   
   paintCurrentDirContents();
   
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
	if (m_dir)
		delete m_dir;

   delete imlIcons;
}

void ISOBrowser::onListActivated(wxListEvent &event)
{
	wxString eventText = event.GetText();
	long     eventData = event.GetData();

	// 
	// let's go back a dir if that's what they want
	// 
	if (eventData == -1)
	{
		m_dir->changeDirectory("..");

		// 
		// move back up the folder tree
		// 
		m_currentTreeRoot = tvwMain->GetItemParent(m_currentTreeRoot);
		tvwMain->SelectItem(m_currentTreeRoot);

		paintCurrentDirContents();
	}
	else if (eventData == 0)
	{
		m_dir->changeDirectory(m_focusedFile.c_str());

		// 
		// set our new tree root and select it
		// 
		m_currentTreeRoot = findTreeItem(m_currentTreeRoot, m_focusedFile);
		tvwMain->SelectItem(m_currentTreeRoot, true);

		// 
		// paint the contents of our new directory
		// 
		paintCurrentDirContents();
	}
	else if (eventData == 1)
	{
		// NOTE: Some images are actually ".cel" or have no extension at all
		//       so I'll allow them to just attempt to view antyhing.
		ImageViewer* imageViewer;
		imageViewer = new ImageViewer(this, m_fileName, wxString::Format("%s%s", m_dir->getPath(), m_focusedFile));
		imageViewer->Show();		
	}
	else
	{
		wxMessageBox(wxString::Format("don't know what to do with file type %d", eventData));
		return;
	}
}

void ISOBrowser::onListFocused(wxListEvent &event)
{
	// 
	// store this so when we handle right click events 
	// we know what file name we're dealing with
	// there's probably a better way to do this, but i don't
	// know what it is
	// 
	m_focusedFile = event.GetText();
}

void ISOBrowser::onListRightClick(wxListEvent &event)
{
	wxString eventText = event.GetText();
	long     eventData = event.GetData();

	if (eventData == 1)
	{
		wxMenu menu;

		menu.Append(ID_MENU_OPEN_IMAGE, "Open as &image");
		menu.Append(ID_MENU_OPEN_CODE, "Open as &code");
		menu.Append(ID_MENU_OPEN_TEXT, "Open as &text");

		PopupMenu(&menu);
	}
}

void ISOBrowser::onPopupMenuOpenImage(wxCommandEvent &event)
{
	// NOTE: Some images are actually ".cel" or have no extension at all
	//       so I'll allow them to just attempt to view antyhing.
	ImageViewer* imageViewer;
	imageViewer = new ImageViewer(this, m_fileName, wxString::Format("%s%s", m_dir->getPath(), m_focusedFile));
	imageViewer->Show();		
}

void ISOBrowser::onPopupMenuOpenCode(wxCommandEvent &event)
{
	CodeViewer* codeViewer;
	codeViewer = new CodeViewer(this, m_fileName, wxString::Format("%s%s", m_dir->getPath(), m_focusedFile));
	codeViewer->Show(true);		
}

void ISOBrowser::onPopupMenuOpenText(wxCommandEvent &event)
{
	wxMessageBox("show text");
}

void ISOBrowser::paintCurrentDirContents()
{
	DirectoryEntry  de;
	int             image;
	long            newItem;

	// 
	// clear our directory listing first
	// 
	lvwMain->DeleteAllItems();
	tvwMain->DeleteChildren(m_currentTreeRoot);

	// 
	// entry for '..' so we can go back if desired
	// but only if we aren't sitting at the root
	// 
	if (strcmp(m_dir->getPath(), "/") != 0)
	{
		newItem = lvwMain->InsertItem (lvwMain->GetItemCount (), _T(".."), IMAGE_ID_FOLDER);
		lvwMain->SetItem (newItem, 1, wxString::Format ("%d", 0));
		lvwMain->SetItem (newItem, 2, wxString::Format ("%d", 0));
		lvwMain->SetItem (newItem, 3, wxString::Format ("%d", 0));
		lvwMain->SetItem (newItem, 4, wxString::Format ("%d", 0));
		lvwMain->SetItem (newItem, 5, wxString (""));
		lvwMain->SetItem (newItem, 6, wxString::Format ("%d", 0));
		lvwMain->SetItem (newItem, 7, wxString::Format ("%d", 0));
		lvwMain->SetItem (newItem, 8, wxString::Format ("%d", 0));
		lvwMain->SetItem (newItem, 9, wxString::Format ("%d", 0));

		// data to sort by later. this one gets a lower value so 
		// it will sort to the top
		lvwMain->SetItemData(newItem, -1);
	}

	while (m_dir->enumerateDirectory (&de))
	{
		if ((de.flags & DirectoryEntryTypeMask) == DirectoryEntryTypeFolder)
		{
			// Folder.
			image = IMAGE_ID_FOLDER;
			tvwMain->AppendItem (m_currentTreeRoot, wxString (de.fileName), image);
		}
		else
		{
			// File!
			image = IMAGE_ID_FILE;
		}

		// Always append to the listview.
		newItem = lvwMain->InsertItem (lvwMain->GetItemCount (), wxString (de.fileName), image);
		lvwMain->SetItem (newItem, 1, wxString::Format ("%d", de.blockSize));
		lvwMain->SetItem (newItem, 2, wxString::Format ("%d", de.copies));
		lvwMain->SetItem (newItem, 3, wxString::Format ("%d", de.entryLengthBlocks));
		lvwMain->SetItem (newItem, 4, wxString::Format ("%d", de.entryLengthBytes));
		lvwMain->SetItem (newItem, 5, wxString (de.ext));
		lvwMain->SetItem (newItem, 6, wxString::Format ("%d", de.flags));
		lvwMain->SetItem (newItem, 7, wxString::Format ("%d", de.gap));
		lvwMain->SetItem (newItem, 8, wxString::Format ("%d", de.id));
		lvwMain->SetItem (newItem, 9, wxString::Format ("%d", de.lastCopy));

		// data to sort by later
		lvwMain->SetItemData(newItem, image);
	}

	// 
	// separate dirs and files
	// 
	lvwMain->SortItems(listSortFunction, 0);

	// 
	// expand the current tree root if it isn't already
	// 
	tvwMain->Expand(m_currentTreeRoot);

	for (int x = 0; x < lvwMain->GetColumnCount(); x++)
	{
		lvwMain->SetColumnWidth (x, wxLIST_AUTOSIZE_USEHEADER);
	}
}

wxTreeItemId ISOBrowser::findTreeItem(wxTreeItemId root, const wxString folder)
{
	wxTreeItemId      ret;
	wxTreeItemIdValue cookie;

	ret = tvwMain->GetFirstChild(root, cookie);

	while (ret.IsOk())
	{
		if (tvwMain->GetItemText(ret) == folder)
			break;

		ret = tvwMain->GetNextChild(root, cookie);
	}

	return ret;
}
