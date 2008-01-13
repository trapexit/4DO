#ifndef _CODEVIEWER_H_
#define _CODEVIEWER_H_

#include "wx/wx.h"
#include "wx/grid.h"

#include "filesystem\file.h"

class CodeViewer : public wxFrame
{
	public:
		CodeViewer(wxFrame* parent, wxString isoPath, wxString filePath);
		CodeViewer(wxFrame* parent, wxString filePath);
		~CodeViewer();

		void initGrid();
		void viewCode(uint8_t *instructions, const uint32_t length);
	private:
		wxGrid *m_grid;
};

#endif // _CODEVIEWER_H_
