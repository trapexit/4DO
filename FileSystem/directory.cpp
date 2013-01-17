// 
// TODO: empty directories? specifically related to the endOfDir var
// 

#include "directory.h"
#include "wx/log.h"
#include "wx/tokenzr.h"

#include <stdio.h>

Directory::Directory(const char *rom)
{
	// 
	// mount the filesystem
	// 
	fileSystem.mount(rom);

	memset(&directoryHeader, 0, sizeof(directoryHeader));

	// 
	// seems like a decent thing to do.  keeps you from
	// enumerating with no directory open at least
	// 
	endOfDir = true;
}

Directory::~Directory()
{
	DirectoryEntry *dirEntry;

	endOfDir = true;
	fileSystem.unmount();

	// 
	// clean up our directory tree
	// 
	while (!dirTree.empty() && (dirEntry = dirTree.front()))
	{
		dirTree.pop_front();
		delete dirEntry;
	}
}

bool Directory::openDirectory(const char *path)
{
	bool              ret;
	wxString          separator(_T("/"));
	wxString          token;
	wxStringTokenizer tokenizer(reinterpret_cast<const wxChar *>(path), separator);
	VolumeHeader      volumeHeader;
	DirectoryEntry    dirEntry, *newEntry, *oldEntry;

	// 
	// can't do much with an empty path
	// 
	if (strlen(path) <= 0)
	{
		wxLogMessage(_T("openDirectory(): path is empty"));
		return false;
	}

	// 
	// if the path isn't absolute, then this isn't going to work
	// 
	if (path[0] != '/')
	{
		wxLogMessage(_T("openDirectory(): path is not absolute, openDirectory only takes absolute paths"));
		return false;
	}

	// 
	// check if our path is somewhat sane
	// 
	if (!tokenizer.HasMoreTokens())
	{
		wxLogMessage(_T("openDirectory(): couldn't find any /'s in the path"));
		return false;
	}

	// 
	// seek to the beginning of the filesystem
	// 
	ret = fileSystem.seekToByte(0, false);

	if (!ret)
	{
		wxLogMessage(_T("openDirectory(): could not seek to the beginning of the filesystem"));
		return false;
	}

	// clear our dir tree
	while (!dirTree.empty() && (oldEntry = dirTree.front()))
	{
		dirTree.pop_front();
		delete oldEntry;
	}

	// 
	// read the volume info from the filesystem
	// 
	ret = fileSystem.readVolumeHeader(&volumeHeader);

	if (!ret)
	{
		wxLogMessage(_T("openDirectory(): could not read volume header from the filesystem"));
		return false;
	}

	// 
	// read the root directory's header
	// 
	ret = fileSystem.readDirectoryHeader(&directoryHeader);

	if (!ret)
	{
		wxLogMessage(_T("openDirectory(): could not read directory header from the filesystem"));
		return false;
	}

	// 
	// create a directory entry for the root dir and throw it into our dir tree
	// 
	newEntry = new DirectoryEntry;
	memset(newEntry, 0, sizeof(DirectoryEntry));

	newEntry->blockSize = volumeHeader.rootDirBlockSize;
	newEntry->entryLengthBlocks = volumeHeader.rootDirBlocks;
	strncpy((char *)newEntry->fileName, "/", 2);
	newEntry->lastCopy = volumeHeader.lastRootDirCopy;
	newEntry->copies = volumeHeader.rootDirCopies[0];

	dirTree.push_front(newEntry);
	
	// 
	// our current path
	// 
	this->path = _T("/");

	while (tokenizer.HasMoreTokens())
	{
		token = tokenizer.GetNextToken();

		if (token.IsEmpty())
			continue;

		ret = findInCurrentDirectory(reinterpret_cast<const char *>(token.c_str()), &dirEntry);

		if (!ret)
		{
			wxLogMessage(
				_T("openDirectory(): findInCurrentDirectory failed for path element %s"),
				token.c_str());
			break;
		}
	}

	// enable enumeration
	if (ret)
		endOfDir = false;

	return true;
}

bool Directory::closeDirectory()
{
	return false;
}

bool Directory::changeDirectory(const char *path)
{
	DirectoryEntry dirEntry;
	wxString       pathString(reinterpret_cast<const wxChar *>(path)), token;
	bool           ret;

	if (pathString.IsEmpty())
		return false;

	// 
	// if our path start with /, then it's an absolute
	// path and we'll just call openDirectory
	// 
	if (pathString.StartsWith(_T("/")))
	{
		wxLogMessage(_T("changeDirectory(): path starts with '/'. calling openDirectory"));
		return openDirectory(path);
	}

	do
	{
		token = pathString.BeforeFirst('/');

		ret = findInCurrentDirectory(reinterpret_cast<const char *>(token.c_str()), &dirEntry);

		if (!ret)
		{
			wxLogMessage(_T("changeDirectory(): findInCurrentDirectory failed for token %s"), token.c_str());
			return false;
		}

		pathString = pathString.AfterFirst('/');
	} while (pathString.Length());

	return true;
}

const char *Directory::getPath()
{
	return reinterpret_cast<const char *>(path.c_str());
}

bool Directory::enumerateDirectory(DirectoryEntry *de)
{
	bool ret;

	// 
	// this is only set if enumerateDirectory has previously seen
	// a directory entry with with a DirectoryEntryPosLastInDir mask
	// 
	if (endOfDir)
	{
		wxLogMessage(_T("enumerateDirectory(): no more directories left to enumerate."));
		return false;
	}

	ret = fileSystem.readDirectoryEntry(de);

	if (!ret)
	{
		wxLogMessage(_T("enumerateDirectory(): readDirectoryEntry failed."));
		return false;
	}
	else if ((de->flags & DirectoryEntryPosMask) == DirectoryEntryPosLastInBlock)
	{
		wxLogMessage(
			_T("enumerateDirectory(): found last entry in block, block size is %d, unused offset is %d"),
			fileSystem.getBlockSize(),
			directoryHeader.unusedOffset);

		// 
		// move to the beginning of the next directory header
		// 
		ret = fileSystem.seekToByte(
			fileSystem.getBlockSize() - directoryHeader.unusedOffset, 
			true);

		if (!ret)
		{
			wxLogMessage(_T("enumerateDirectory(): failed to seek to beginning of new directory header"));
			return false;
		}

		ret = fileSystem.readDirectoryHeader(&directoryHeader);

		if (!ret)
		{
			wxLogMessage(_T("enumerateDirectory(): failed to read the next directory header"));
			return false;
		}

		return true;
	}
	else if ((de->flags & DirectoryEntryPosMask) == DirectoryEntryPosLastInDir)
	{
		wxLogMessage(_T("enumerateDirectory(): found last directory entry in the directory."));
		endOfDir = true;
	}

	return true;
}

bool Directory::findInCurrentDirectory(const char *item, DirectoryEntry *dirEntry)
{
	bool           ret, found = false;
	DirectoryEntry *newEntry, *currentEntry;

	// 
	// can't do much of anything if we don't have a current directory
	// 
	if (dirTree.empty())
	{
		wxLogMessage(_T("findInCurrentDirectory(): find anything without an open directory"));
		return false;
	}

	// 
	// if the desired directory is '..', then we want to go up a dir.
	// get the parent dir and adjust the current path
	// 

	if (strcmp(item, "..") == 0)
	{
		DirectoryEntry *currentEntry = dirTree.front();
		dirTree.pop_front();
		newEntry = dirTree.front();

		ret = fileSystem.seekToBlock(newEntry->copies, false);

		if (!ret)
		{
			wxLogMessage(_T("changeDirectory(): couldn't seek to directory %s"), (char *)newEntry->fileName);
			dirTree.push_front(currentEntry);
			return false;
		}

		ret = fileSystem.readDirectoryHeader(&directoryHeader);

		if (!ret)
		{
			wxLogMessage(_T("changeDirectory: found directory %s but failed to read it's header"), (char *)newEntry->fileName);
			dirTree.push_front(currentEntry);
			return false;
		}

		// remove the rightmost path element from our current path
		path = path.Left(path.Length() - (strlen((char *)currentEntry->fileName) + 1));

		// cleanup
		if (currentEntry)
			delete currentEntry;

		endOfDir = false;

		return true;
	}

	// 
	// move the filesystem fp to the beginning of the dir and reset
	// the end of directory indicator so we can enumerate
	// 

	currentEntry = dirTree.front();
	ret = fileSystem.seekToBlock(currentEntry->copies, false);

	if (!ret)
	{
		wxLogMessage(
			_T("findInCurrentDirectory(): couldn't move to beginning of directory %s"), 
			(char *)currentEntry->fileName);
		return false;
	}

	ret = fileSystem.readDirectoryHeader(&directoryHeader);

	if (!ret)
	{
		wxLogMessage(
			_T("findInCurrentDirectory(): couldn't read directory header for %s"),
			(char *)currentEntry->fileName);
		return false;
	}

	endOfDir = false;

	while (enumerateDirectory(dirEntry))
	{
		if (strcmp((char *)dirEntry->fileName, item) == 0)
		{
			// 
			// we found what we wanted.
			// move to the beginning of the directory and read the header
			// 

			ret = fileSystem.seekToBlock(dirEntry->copies, false);

			if (!ret)
			{
				wxLogMessage(_T("changeDirectory(): couldn't seek to directory %s"), item);
				break;
			}

			ret = fileSystem.readDirectoryHeader(&directoryHeader);

			if (!ret)
			{
				wxLogMessage(_T("changeDirectory: found directory %s but failed to read it's header"), item);
				break;
			}

			// update the directory tree
			newEntry = new DirectoryEntry;
			memcpy(newEntry, dirEntry, sizeof(DirectoryEntry));

			dirTree.push_front(newEntry);

			// update our current path
			path.Append(reinterpret_cast<const wxChar *>(item));
			path.Append(_T("/"));

			found = true;
			break;
		}
	}

	if (found)
		endOfDir = false;

	return found;
}
