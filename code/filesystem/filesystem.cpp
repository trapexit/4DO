// 
// TODO: throw some debug logging in here
// TODO: maybe use relative file positioning instead of 
//       starting from the beginning every time
// 

#include "filesystem.h"
#include "wx/log.h"

#include <stdio.h>

FileSystem::FileSystem()
{
	imageName = NULL;

	memset(&volumeHeader, 0, sizeof(volumeHeader));
}

FileSystem::~FileSystem()
{
	if (imageName)
		delete imageName;
}

bool FileSystem::mount(const char *path)
{
	bool ret;

	ret = file.Open((const wxChar *)path);

	if (!ret)
	{
		wxLogMessage("FileSystem::mount(): couldn't open iso located at %s", path);
		return false;
	}

	ret = readVolumeHeader(&volumeHeader);

	if (!ret)
	{
		wxLogMessage("FileSystem::mount(): could not read volume header from the filesystem");
		return false;
	}

	imageName = _strdup(path);

	if (!imageName)
	{
		wxLogMessage("FileSystem::mount(): could not allocate memory for a copy of the image name");
		return false;
	}

	return true;
}

void FileSystem::unmount()
{
	file.Close();
}

bool FileSystem::readVolumeHeader(VolumeHeader *vh)
{
	uint32_t bytesRead;
	bool     ret;

	ret = read(vh, volumeHeaderSize, &bytesRead);

	if (!ret)
	{
		wxLogMessage("FileSystem::readVolumeHeader: couldn't read volume header");
		return false;
	}

	// 
	// make all relevant fields little endian
	// 

	endianSwap(vh->id);
	endianSwap(vh->blockSize);
	endianSwap(vh->blockCount);
	endianSwap(vh->rootDirId);
	endianSwap(vh->rootDirBlocks);
	endianSwap(vh->rootDirBlockSize);
	endianSwap(vh->lastRootDirCopy);
	for (int i = 0; i < 8; i++)
		endianSwap(vh->rootDirCopies[i]);
	
	// 
	// go ahead and move the fp forward to the root dir location
	// 

	ret = seekToBlock(vh->rootDirCopies[0], false);

	if (!ret)
		return false;

	return true;
}

bool FileSystem::readDirectoryHeader(DirectoryHeader *dh)
{
	uint32_t bytesRead;
	bool     ret;

	ret = read(dh, directoryHeaderSize, &bytesRead);

	if (!ret)
	{
		wxLogMessage("FileSystem::readDirectoryHeader(): couldn't read directory header");
		return false;
	}

	// 
	// make all relevant fields little endian
	// 

	endianSwap(dh->nextBlock);
	endianSwap(dh->prevBlock);
	endianSwap(dh->flags);
	endianSwap(dh->unusedOffset);
	endianSwap(dh->directoryOffset);

	return true;
}

bool FileSystem::readDirectoryEntry(DirectoryEntry *de)
{
		uint32_t bytesRead;
		bool     ret;

		ret = read(de, directoryEntrySize, &bytesRead);

		if (!ret)
		{
			wxLogMessage("FileSystem::readDirectoryEntry(): couldn't read directory entry");
			return false;
		}

		// 
		// make all relevant fields little endian
		//

		endianSwap(de->flags);
		endianSwap(de->id);
		endianSwap(de->blockSize);
		endianSwap(de->entryLengthBytes);
		endianSwap(de->entryLengthBlocks);
		endianSwap(de->burst);
		endianSwap(de->gap);
		endianSwap(de->lastCopy);
		endianSwap(de->copies);

		// 
		// we may need to move the file pointer a little further along.
		// this is due to the fact that the copies field is actually of 
		// variable length, but we always only read in 4 bytes of it
		// because we don't really need more than one copy of anything.
		// 

		if (de->lastCopy)
		{
			int32_t pos = de->lastCopy * 4;

			ret = seekToByte(pos, true);

			if (!ret)
				return false;
		}

		return true;
}

bool FileSystem::read(void *buf, const uint32_t bufLength, uint32_t *bytesRead)
{
	size_t ret;

	if (!bufLength)
	{
		wxLogMessage("FileSystem::read(): bufLength size of %d is invalid", bufLength);
		return false;
	}

	ret = file.Read(buf, bufLength);

	if (ret == wxInvalidOffset)
	{
		wxLogMessage("FileSystem::read(): failed to read from filesystem: %d", ret);
		return false;
	}

	return true;
}

bool FileSystem::seekToBlock(const uint32_t block, const bool relative)
{
	bool ret;
	uint32_t pos = (volumeHeader.blockSize * block);

	ret = seekToByte(pos, relative);

	if (!ret)
	{
		wxLogMessage(
			"FileSystem::seekToBlock(): couldn't set file pointer position of %d", 
			pos);
		return false;
	}

	return true;
}

bool FileSystem::seekToByte(const uint32_t byte, const bool relative)
{
	wxFileOffset ret, pos = byte;
	
	wxLogDebug("FileSystem::seekToByte(): moving fp forward %d bytes", pos);

	ret = file.Seek(pos, (relative ? wxFromCurrent : wxFromStart));

	if (ret == wxInvalidOffset)
	{
		wxLogMessage(
			"FileSystem::seekToBlock(): couldn't set file pointer position of %d", 
			pos);
		return false;
	}

	return true;
}

uint32_t FileSystem::getBlockSize()
{
	// XXX: is this always true?
	return volumeHeader.blockSize;
}

const char *FileSystem::getImageName()
{
	return imageName;
}

void FileSystem::printVolumeHeader(const VolumeHeader *vh)
{
	wxLogMessage("recordType       = %02x", vh->recordType);
	wxLogMessage("syncBytes        = ");
	for (int i = 0; i < 5; i++)
		wxLogMessage("%02x", vh->syncBytes[i]);
	wxLogMessage("");
	wxLogMessage("recordVersion    = %02x", vh->recordVersion);
	wxLogMessage("volumeFlags      = %02x", vh->flags);
	wxLogMessage("volumeComment    = ");
	for (int i = 0; i < 32; i++)
		wxLogMessage("%c", vh->comment[i]);
	wxLogMessage("");
	wxLogMessage("volumeLabel      = ");
	for (int i = 0; i < 32; i++)
		wxLogMessage("%c", vh->label[i]);
	wxLogMessage("");
	wxLogMessage("volumeId         = %08x", vh->id);
	wxLogMessage("blockSize        = %08x (%d bytes)", vh->blockSize, vh->blockSize);
	wxLogMessage("blockCount       = %08x (%d KB in volume)", vh->blockCount, (vh->blockCount * vh->blockSize) / 1024);
	wxLogMessage("rootDirId        = %08x", vh->rootDirId);
	wxLogMessage("rootDirBlocks    = %08x (%d blocks)", vh->rootDirBlocks, vh->rootDirBlocks);
	wxLogMessage("rootDirBlockSize = %08x (%d bytes)", vh->rootDirBlockSize, vh->rootDirBlockSize);
	wxLogMessage("lastRootDirCopy  = %08x", vh->lastRootDirCopy);
	wxLogMessage("rootDirCopies    = ");
	for (int i = 0; i < 8; i++)
	{
		wxLogMessage("%08x", vh->rootDirCopies[i]);

		if (i + 1 < 8)
			wxLogMessage(" ");
	}
	wxLogMessage("");
}

void FileSystem::printDirectoryHeader(const DirectoryHeader *dh)
{
	wxLogMessage("nextBlock       = %08x", dh->nextBlock);
	wxLogMessage("prevBlock       = %08x", dh->prevBlock);
	wxLogMessage("flags           = %08x", dh->flags);
	wxLogMessage("unusedOffset    = %08x", dh->unusedOffset);
	wxLogMessage("directoryOffset = %08x", dh->directoryOffset);
}

void FileSystem::printDirectoryEntry(const DirectoryEntry *de)
{
	wxLogMessage("flags             = %08x", de->flags);
	wxLogMessage("id                = %08x", de->id);
	wxLogMessage("ext               = ");
	for (int i = 0; i < 4; i++)
		wxLogMessage("%c", de->ext[i]);
	wxLogMessage("");
	wxLogMessage("blockSize         = %08x", de->blockSize);
	wxLogMessage("entryLengthBytes  = %08x", de->entryLengthBytes);
	wxLogMessage("entryLengthBlocks = %08x", de->entryLengthBlocks);
	wxLogMessage("burst             = %08x", de->burst);
	wxLogMessage("gap               = %08x", de->gap);
	wxLogMessage("fileName          = ");
	for (int i = 0; i < 32; i++)
		wxLogMessage("%c", de->fileName[i]);
	wxLogMessage("");
	wxLogMessage("lastCopy          = %08x", de->lastCopy);
	wxLogMessage("copies            = %08x", de->copies);
}

void FileSystem::printString(const char *str)
{
	printString((uint8_t *)str, strlen(str));
}

void FileSystem::printString(const uint8_t *str, const uint32_t strLength)
{
	for (int i = 0; i < strLength; i++)
	{
		// 
		// files in the opera filesystem don't have line feeds, just
		// carriage returns.  in the name of readability, let's add
		// a line feed before every carriage return
		// 
		if (str[i] == 0x0D)
			wxLogMessage("");

		wxLogMessage("%c", str[i]);
	}

	wxLogMessage("");
}

void FileSystem::endianSwap(uint16_t &x)
{
	x = (x >> 8) | (x << 8);
}

void FileSystem::endianSwap(uint32_t &x)
{
	x = (x >> 24) | 
	    ((x >> 8) & 0x0000FF00) | 
	    ((x << 8) & 0x00FF0000) | 
	    (x << 24);
}

void FileSystem::endianSwap(int32_t &x)
{
	x = (x >> 24) | 
	    ((x >> 8) & 0x0000FF00) | 
	    ((x << 8) & 0x00FF0000) | 
	    (x << 24);
}
