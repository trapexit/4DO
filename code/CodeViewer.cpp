#include "CodeViewer.h"
#include "Console.h"

#include "wx/file.h"

/////////////////////////////////////////////////////////////////////////
// Frame startup
/////////////////////////////////////////////////////////////////////////
CodeViewer::CodeViewer(wxFrame* parent, wxString isoPath, wxString filePath)
      : wxFrame (parent, -1, wxEmptyString, wxDefaultPosition, wxDefaultSize)
{
	const uint32_t length = 500;
	uint32_t       bytesRead;
	uint8_t        instructions[length];
	bool           ret;
	File           file(isoPath.c_str());

	ret = file.openFile(filePath.c_str());

	if (!ret)
	{
		wxMessageBox(wxString::Format("Couldn't open %s for display", filePath));
		return;
	}

	ret = file.read(instructions, length, &bytesRead);

	if (!ret)
	{
		wxMessageBox(wxString::Format("Couldn't read from file %s", filePath));
		return;
	}

	this->SetTitle (wxString().Format ("Code Viewer - %s", filePath));

	// 
	// initialize the grid that will display the code
	// 
	initGrid();

	// 
	// parse the instructions we read from the file and display then
	// 
	viewCode(instructions, length);
}

CodeViewer::CodeViewer(wxFrame* parent, wxString filePath)
      : wxFrame (parent, -1, wxEmptyString, wxDefaultPosition, wxDefaultSize)
{
	const uint32_t length = 500;
	uint8_t        instructions[length];
	size_t         bytesRead;
	bool           ret;
	wxFile         file;

	ret = file.Open(filePath);

	if (!ret)
	{
		wxMessageBox(wxString::Format("Couldn't open %s for display", filePath));
		return;
	}

	bytesRead = file.Read(instructions, length);

	if (!bytesRead)
	{
		wxMessageBox(wxString::Format("Couldn't read from file %s", filePath));
		return;
	}

	this->SetTitle (wxString().Format ("Code Viewer - %s", filePath));

	// 
	// initialize the grid that will display the code
	// 
	initGrid();

	// 
	// parse the instructions we read from the file and display then
	// 
	viewCode(instructions, length);
}

CodeViewer::~CodeViewer()
{
	if (m_grid)
		delete m_grid;
}

void CodeViewer::initGrid()
{
	/////////////////////////
	m_grid = new wxGrid(this, -1, wxDefaultPosition, wxDefaultSize);
   
	/////////////////
	// Setup grid.
	m_grid->CreateGrid (0, 3, wxGrid::wxGridSelectCells);
	m_grid->EnableDragRowSize (false);
	m_grid->EnableEditing (false);
	m_grid->SetColLabelValue (0, "Cnd");
	m_grid->SetColLabelValue (1, "Instruction");
	m_grid->SetColLabelValue (2, "Last CPU Result");
   
	this->SetSize (640, 480);
	this->CenterOnScreen ();
	this->SetBackgroundColour (wxColor (0xFF000000));
}

void CodeViewer::viewCode(uint8_t *instructions, const uint32_t length)
{
	wxString  bits;
	Console*  con;

	con = new Console ();

	for (uint row = 0; row < length; row++)
	{
		// Read an instruction.
		uint token;
		token = (instructions[(row * 4)] << 24) + (instructions[(row * 4 + 1)] << 16) + (instructions[(row * 4 + 2)] << 8) + instructions[(row * 4 + 3)];
		bits = UintToBitString (token);

		// Write it memory temporarily...
		con->DMA ()->SetValue (row * 4, token);
		// Set PC there.
		*(con->CPU ()->REG->PC ()->Value) = row * 4;
		// Process it.
		con->CPU ()->DoSingleInstruction ();

		//////////////
		// Make a new row.
		wxString cond;

		cond = wxString::Format ("%s (%s)",  bits.Mid (0, 4), con->CPU ()->LastCond);
		m_grid->InsertRows (m_grid->GetRows ());
		m_grid->SetCellValue (row, 0, cond);
		m_grid->SetCellValue (row, 1, bits.Mid (4));
		//m_grid->SetCellValue (row, 2, con->CPU ()->LastResult);
		m_grid->SetRowLabelValue (row, wxString::Format ("%d", row));
	}

	/////////////////////
	// Auto size columns.
	m_grid->AutoSizeColumns ();

	delete con;
}
