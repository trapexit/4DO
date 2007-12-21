// 
// TODO: empty directories? specifically related to the endOfDir var
// 

#include "directory.h"
#include <stdio.h>

Directory::Directory(const char *rom)
{
	// 
	// mount the filesystem
	// 
	fileSystem.mount(rom);

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
	DirectoryHeader dirHeader;
	DirectoryEntry  dirEntry;

	if (!strlen(path))
	{
		printf("openDirectory(): path is non-existent\n");
		return false;
	}

	dirPath = _strdup(path);

	if (!dirPath)
	{
		printf("openDirectory(): _strdup failed to allocate memory\n");
		return false;
	}

	ret = fileSystem.readDirectoryHeader(&dirHeader);

	if (!ret)
	{
		printf("openDirectory(): could not read directory header from the filesystem\n");
		return false;
	}

	fileSystem.printDirectoryHeader(&dirHeader);

	// TODO: relative paths
	token = strtok_s(dirPath, separator, &context);

	if (!token)
	{
		// if our path is /, just return, there's nothing else to do
		if (strcmp(dirPath, separator) == 0)
		{
			endOfDir = false;
			return true;
		}

		printf("openDirectory(): couldn't find any /'s in the path\n");
		return false;
	}

	for (;;)
	{
		ret = fileSystem.readDirectoryEntry(&dirEntry);

		if (!ret)
		{
			printf("openDirectory(): could not read directory entry from the filesystem\n");
			return false;
		}

		fileSystem.printDirectoryEntry(&dirEntry);

		if (strcmp((char *)dirEntry.fileName, token) == 0)
		{
			// 
			// we found what we wanted, seek to the directory and
			// find the next token if there is one
			// 

			ret = fileSystem.seekToBlock(dirEntry.copies, false);

			if (!ret)
			{
				printf("openDirectory(): couldn't seek to directory %s\n", token);
				return false;
			}

			ret = fileSystem.readDirectoryHeader(&directoryHeader);

			if (!ret)
			{
				printf("openDirectory: found the directory but failed to read it's header\n");
				return false;
			}

			fileSystem.printDirectoryHeader(&directoryHeader);

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
				printf("enumerateDirectory(): failed to seek to beginning of new directory header\n");
				return false;
			}

			ret = fileSystem.readDirectoryHeader(&directoryHeader);

			if (!ret)
			{
				printf("enumerateDirectory(): failed to read the next directory header\n");
				return false;
			}

			FileSystem::printDirectoryHeader(&directoryHeader);
		}
		else if ((dirEntry.flags & DirectoryEntryPosMask) == DirectoryEntryPosLastInDir)
		{
			printf(
				"openDirectory(): couldn't find the path element %s, bailing out\n",
				token);
			return false;
		}
	}

	// enable enumeration
	endOfDir = false;

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
		printf("enumerateDirectory(): no more directories left to enumerate.\n");
		return false;
	}

	ret = fileSystem.readDirectoryEntry(de);

	if (!ret)
	{
		printf("enumerateDirectory(): readDirectoryEntry failed.\n");
		return false;
	}
	else if ((de->flags & DirectoryEntryPosMask) == DirectoryEntryPosLastInBlock)
	{
		// 
		// move to the beginning of the next directory header
		// 
		ret = fileSystem.seekToByte(
			fileSystem.getBlockSize() - directoryHeader.unusedOffset, 
			true);

		if (!ret)
		{
			printf("enumerateDirectory(): failed to seek to beginning of new directory header\n");
			return false;
		}

		ret = fileSystem.readDirectoryHeader(&directoryHeader);

		if (!ret)
		{
			printf("enumerateDirectory(): failed to read the next directory header\n");
			return false;
		}

		FileSystem::printDirectoryHeader(&directoryHeader);
		return true;
	}
	else if ((de->flags & DirectoryEntryPosMask) == DirectoryEntryPosLastInDir)
	{
		printf("enumerateDirectory(): found last directory entry in the directory.\n");
		endOfDir = true;
	}

	return true;
}
