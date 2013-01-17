#include "file.h"
#include "wx/log.h"

#include <stdio.h>

#ifdef GENRE_UNIX
	#define _strdup strdup
#endif

File::File(const char *rom)
{
	VolumeHeader vh;

	// 
	// mount the filesystem
	// 
	fileSystem.mount(rom);

	// 
	// read the volume header first
	// 
	fileSystem.readVolumeHeader(&vh);

	memset(&dirEntry, 0, sizeof(dirEntry));

	// 
	// seems like a decent thing to do. 
	// 
	endOfFile = true;
}

File::~File()
{
	endOfFile = true;
	fileSystem.unmount();
}

bool File::openFile(const char *path)
{
	bool       ret;
	char       *fileName, *filePath, *fileNameStart;
	// TODO: make separator a member of the filesystem?
	char       separator = '/';
	Directory  dir(fileSystem.getImageName());

	filePath = _strdup(path);

	if (!filePath)
	{
		wxLogMessage(_T("openFile(): _strdup failed to allocate memory"));
		return false;
	}

	for (;;)
	{
		//
		// find the last / in the path.  we will split the string based
		// on this.  we will then have '/path/to/file/dir' and 'file_name'
		// 
		fileNameStart = strrchr(filePath, separator);

		// TODO: file in / and relative paths
		if (!fileNameStart || !strlen(fileNameStart))
		{
			wxLogMessage(_T("openFile(): didn't find any /'s in the path or no filename in the path"));
			ret = false;
			break;
		}

		// the ++ is to move past the last /
		fileName = _strdup(++fileNameStart);

		if (!fileName)
		{
			wxLogMessage(_T("openFile(): _strdup failed to allocate memory"));
			ret = false;
			break;
		}

		// 
		// shorten the filepath down to whatever is before the actual
		// file name. 
		// 
		*fileNameStart = 0;

		// 
		// open the directory that contains our file
		// 

		ret = dir.openDirectory(filePath);

		if (!ret)
		{
			wxLogMessage(_T("openFile(): openDirectory failed"));
			break;
		}

		// 
		// enumerate through the directory contents to find our file
		// 
		while (dir.enumerateDirectory(&dirEntry))
		{
			// 
			// we found the file we're looking for, seek to the file
			// location in the filesystem and then we should be ready
			// to start reading from the file
			// 
			if (strcmp((char *)dirEntry.fileName, fileName) == 0)
			{
				ret = fileSystem.seekToBlock(dirEntry.copies, false);

				if (!ret)
				{
					wxLogMessage(_T("openFile(): could not seek to block %08x to open file"), dirEntry.copies);
					break;
				}

				// 
				// we're ready to read from the file
				// 
				endOfFile = false;
				break;
			}
		}

		break;
	}

	if (filePath)
		delete[] filePath;
	
	return ret;
}

bool File::closeFile()
{
	return false;
}

bool File::read(uint8_t *buf, const uint32_t bufLength, uint32_t *bytesRead)
{
	uint32_t bytesToRead = bufLength;

	if (endOfFile)
	{
		wxLogMessage(_T("read(): end of file"));
		return false;
	}

	// 
	// we don't want to read past the end of the file entry
	// 
	if (bytesToRead > (dirEntry.entryLengthBytes - currBytes))
		bytesToRead = (dirEntry.entryLengthBytes - currBytes);

	return fileSystem.read(buf, bytesToRead, bytesRead);
}

bool File::readLine(uint8_t *buf, const uint32_t bufLength, uint32_t *bytesRead)
{
	if (endOfFile)
	{
		wxLogMessage(_T("read(): end of file"));
		return false;
	}

	return false;
}

uint32_t File::getFileSize()
{
	return dirEntry.entryLengthBytes;
}

uint32_t File::getFileType()
{
	return (dirEntry.flags & DirectoryEntryTypeMask);
}

const uint8_t *File::getFileExt()
{
	return dirEntry.ext;
}

const char *File::getFileName()
{
	return (const char *)dirEntry.fileName;
}
