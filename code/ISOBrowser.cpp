#include "ISOBrowser.h"
#include "ImageViewer.h"

enum IDs
{
	ID_LIST_VIEW = 1,
	ID_TREE_VIEW
};

enum imageIds
{
	IMAGE_ID_FOLDER = 0,
	IMAGE_ID_FILE
};

/////////////////////////////////////////////////////////////////////////
// Event declaration
/////////////////////////////////////////////////////////////////////////
BEGIN_EVENT_TABLE(ISOBrowser, wxFrame)
	EVT_LIST_ITEM_ACTIVATED(ID_LIST_VIEW, ISOBrowser::onListActivated)
END_EVENT_TABLE()

/////////////////////////////////////////////////////////////////////////
// Frame startup
/////////////////////////////////////////////////////////////////////////

int wxCALLBACK listSortFunction(long item1, long item2, long WXUNUSED(data))
{
	if (item1 > item2)
		return 1;
	else if (item1 < item2)
		return -1;
	else
		return 0;
}

ISOBrowser::ISOBrowser(wxFrame* parent, wxString fileName)
      : wxFrame (parent, -1, wxEmptyString, wxDefaultPosition, wxDefaultSize)
{
   this->SetTitle (wxString().Format ("ISO Browser - %s", fileName));
   
   /////////////////////////
   
   // 
   // start out at the root directory
   // 
   m_currentPath = "/";

   m_fileName = fileName;
   
   imlIcons = new wxImageList (16, 16);
   imlIcons->Add (wxIcon (folder_xpm));
   imlIcons->Add (wxIcon (file_xpm));
   
   tvwMain = new wxTreeCtrl (this, ID_TREE_VIEW);
   tvwMain->SetImageList (imlIcons);
   m_currentTreeRoot = tvwMain->AddRoot (_T(m_currentPath), 0);

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
   delete imlIcons;
}

void ISOBrowser::onListActivated(wxListEvent &event)
{
	bool     ret;
	File     f(m_fileName);
	uint32_t type;
	wxString eventText = event.GetText();

	// 
	// let's go back a dir if that's what they want
	// 
	if (eventText == "..")
	{
		// 
		// in a string like /some/path/that/rules/ this will remove
		// everything after the second to last /, leaving /some/path/that/
		// 
		m_currentPath = m_currentPath.BeforeLast('/');
		m_currentPath = m_currentPath.BeforeLast('/') + _T("/");

		// 
		// move back up the folder tree
		// 
		m_currentTreeRoot = tvwMain->GetItemParent(m_currentTreeRoot);
		tvwMain->SelectItem(m_currentTreeRoot);

		paintCurrentDirContents();
		return;
	}

	ret = f.openFile(wxString::Format("%s%s", m_currentPath, eventText).c_str());

	if (!ret)
	{
		wxMessageBox(wxString::Format("couldn't open %s!", eventText));
		return;
	}

	type = f.getFileType();

	if (type == DirectoryEntryTypeFolder)
	{
		// 
		// init our new directory path so we can paint it's contents
		// 
		m_currentPath += eventText + _T("/");

		// 
		// set our new tree root and select it
		// 
		m_currentTreeRoot = findTreeItem(m_currentTreeRoot, eventText);
		tvwMain->SelectItem(m_currentTreeRoot, true);

		// 
		// paint the contents of our new directory
		// 
		paintCurrentDirContents();
	}
	else if (type == DirectoryEntryTypeFile)
	{
		wxString ext(f.getFileExt(), 4);

		// 
		// image
		// 
		if (ext.MakeLower() == "img " || ext.MakeLower() == "imag")
		{
			ImageViewer* imageViewer;
			imageViewer = new ImageViewer(this, m_fileName, wxString::Format("%s%s", m_currentPath, eventText));
			imageViewer->Show();
		}
		// 
		// cel
		// 
		else if (ext == "cel ")
		{
			wxMessageBox(_T("Don't know how to render cels yet"));
			return;
		}
		else
		{
			wxMessageBox(wxString::Format("Don't know how to display %s", eventText));
			return;
		}
	}
	else
	{
		wxMessageBox(wxString::Format("don't know what to do with file type %d", type));
		return;
	}
}

void ISOBrowser::paintCurrentDirContents()
{
	Directory       dir (m_fileName);
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
	if (m_currentPath != "/")
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

	dir.openDirectory (m_currentPath.c_str());
	while (dir.enumerateDirectory (&de))
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
