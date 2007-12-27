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
	EVT_LIST_ITEM_ACTIVATED(ID_LIST_VIEW, ISOBrowser::onActivated)
END_EVENT_TABLE()

/////////////////////////////////////////////////////////////////////////
// Frame startup
/////////////////////////////////////////////////////////////////////////
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
   
   tvwMain = new wxTreeCtrl (this, ID_TREE_VIEW, wxDefaultPosition, wxDefaultSize, wxTR_HIDE_ROOT);
   tvwMain->SetImageList (imlIcons);
   m_currentTreeRoot = tvwMain->AddRoot (_T("My Root"), 0);
   m_currentTreeRoot = tvwMain->AppendItem (m_currentTreeRoot, _T("/"), IMAGE_ID_FOLDER);

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

void ISOBrowser::onActivated(wxListEvent &event)
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

		paintCurrentDirContents();
	}
	else if (type == DirectoryEntryTypeFile)
	{
		wxString ext(f.getFileExt(), 4);

		// 
		// image
		// 
		if (ext == "img " || ext == "IMAG")
		{
			ImageViewer* imageViewer;
			imageViewer = new ImageViewer(this, m_fileName, wxString::Format("%s%s", m_currentPath, eventText));
			imageViewer->Show();
		}
		// 
		// cel
		// 
		else if (ext == "cel")
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

	// 
	// clear our directory listing first
	// 
	lvwMain->DeleteAllItems();

	// 
	// entry for '..' so we can go back if desired
	// but only if we aren't sitting at the root
	// 
	if (m_currentPath != "/")
	{
		lvwMain->InsertItem (lvwMain->GetItemCount (), _T(".."), IMAGE_ID_FOLDER);
		lvwMain->SetItem (lvwMain->GetItemCount () - 1, 1, wxString::Format ("%d", 0));
		lvwMain->SetItem (lvwMain->GetItemCount () - 1, 2, wxString::Format ("%d", 0));
		lvwMain->SetItem (lvwMain->GetItemCount () - 1, 3, wxString::Format ("%d", 0));
		lvwMain->SetItem (lvwMain->GetItemCount () - 1, 4, wxString::Format ("%d", 0));
		lvwMain->SetItem (lvwMain->GetItemCount () - 1, 5, wxString (""));
		lvwMain->SetItem (lvwMain->GetItemCount () - 1, 6, wxString::Format ("%d", 0));
		lvwMain->SetItem (lvwMain->GetItemCount () - 1, 7, wxString::Format ("%d", 0));
		lvwMain->SetItem (lvwMain->GetItemCount () - 1, 8, wxString::Format ("%d", 0));
		lvwMain->SetItem (lvwMain->GetItemCount () - 1, 9, wxString::Format ("%d", 0));
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
}
