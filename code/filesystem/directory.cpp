// 
// TODO: empty directories? specifically related to the endOfDir var
// 

#include "directory.h"
#include "wx/log.h"

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
	endOfDir = true;
	fileSystem.unmount();
}

bool Directory::openDirectory(const char *path)
{
	bool ret;
	char *token, *context, *dirPath;
	// TODO: separator should be in filesystem?
	char separator[] = "/";
	DirectoryEntry  dirEntry;

	if (!strlen(path))
	{
		wxLogMessage("openDirectory(): path is non-existent");
		return false;
	}

	dirPath = _strdup(path);

	if (!dirPath)
	{
		wxLogMessage("openDirectory(): _strdup failed to allocate memory");
		return false;
	}

	ret = fileSystem.readDirectoryHeader(&directoryHeader);

	if (!ret)
	{
		wxLogMessage("openDirectory(): could not read directory header from the filesystem");
		return false;
	}

	// TODO: relative paths
	token = strtok_s(dirPath, separator, &context);

	if (!token)
	{
		// if our path is /, just return, there's nothing else to do
		if (strcmp(dirPath, separator) == 0)
		{
			endOfDir = false;
			delete dirPath;
			return true;
		}

		wxLogMessage("openDirectory(): couldn't find any /'s in the path");
		delete dirPath;
		return false;
	}

	for (;;)
	{
		ret = fileSystem.readDirectoryEntry(&dirEntry);

		if (!ret)
		{
			wxLogMessage("openDirectory(): could not read directory entry from the filesystem");
			break;
		}

		if (strcmp((char *)dirEntry.fileName, token) == 0)
		{
			// 
			// we found what we wanted, seek to the directory and
			// find the next token if there is one
			// 

			ret = fileSystem.seekToBlock(dirEntry.copies, false);

			if (!ret)
			{
				wxLogMessage("openDirectory(): couldn't seek to directory %s", token);
				break;
			}

			ret = fileSystem.readDirectoryHeader(&directoryHeader);

			if (!ret)
			{
				wxLogMessage("openDirectory: found the directory but failed to read it's header");
				break;
			}

			token = strtok_s(NULL, separator, &context);

			// loop until we don't see any more path separators
			if (!token)
				break;
		}
		else if ((dirEntry.flags & DirectoryEntryPosMask) == DirectoryEntryPosLastInBlock)
		{
			// 
			// move to the beginning of the next directory header
			// 
			ret = fileSystem.seekToByte(
				fileSystem.getBlockSize() - directoryHeader.unusedOffset, 
				true);

			if (!ret)
			{
				wxLogMessage("enumerateDirectory(): failed to seek to beginning of new directory header");
				break;
			}

			ret = fileSystem.readDirectoryHeader(&directoryHeader);

			if (!ret)
			{
				wxLogMessage("enumerateDirectory(): failed to read the next directory header");
				break;
			}
		}
		else if ((dirEntry.flags & DirectoryEntryPosMask) == DirectoryEntryPosLastInDir)
		{
			wxLogMessage(
				"openDirectory(): couldn't find the path element %s, bailing out",
				token);
			break;
		}
	}

	// enable enumeration
	endOfDir = false;

	if (dirPath)
		delete dirPath;

	return true;
}

bool Directory::closeDirectory()
{
	return false;
}

bool Directory::changeDirectory(const char *path)
{
	return false;
}

bool Directory::getDirectory(const char *path)
{
	return false;
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
		wxLogMessage("enumerateDirectory(): no more directories left to enumerate.");
		return false;
	}

	ret = fileSystem.readDirectoryEntry(de);

	if (!ret)
	{
		wxLogMessage("enumerateDirectory(): readDirectoryEntry failed.");
		return false;
	}
	else if ((de->flags & DirectoryEntryPosMask) == DirectoryEntryPosLastInBlock)
	{
		wxLogMessage(
			"enumerateDirectory(): found last entry in block, block size is %d, unused offset is %d",
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
			wxLogMessage("enumerateDirectory(): failed to seek to beginning of new directory header");
			return false;
		}

		ret = fileSystem.readDirectoryHeader(&directoryHeader);

		if (!ret)
		{
			wxLogMessage("enumerateDirectory(): failed to read the next directory header");
			return false;
		}

		return true;
	}
	else if ((de->flags & DirectoryEntryPosMask) == DirectoryEntryPosLastInDir)
	{
		wxLogMessage("enumerateDirectory(): found last directory entry in the directory.");
		endOfDir = true;
	}

	return true;
}
