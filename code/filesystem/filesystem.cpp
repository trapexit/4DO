// 
// TODO: change printfs to some logger facility
// TODO: handle directory headers that span blocks
// TODO: maybe use relative file positioning instead of 
//       starting from the beginning every time
// 

#include "filesystem.h"
#include <stdio.h>

FileSystem::FileSystem()
{
	fp = INVALID_HANDLE_VALUE;
	imageName = NULL;

	memset(&volumeHeader, 0, sizeof(volumeHeader));
}

FileSystem::~FileSystem()
{
	if (fp != INVALID_HANDLE_VALUE)
		CloseHandle(fp);
	
	if (imageName)
		delete imageName;
}

bool FileSystem::mount(const char *path)
{
	bool ret;

	fp = CreateFileA(
		path,
		GENERIC_READ,
		FILE_SHARE_READ,
		NULL,
		OPEN_ALWAYS,
		FILE_ATTRIBUTE_NORMAL,
		NULL);

	if (fp == INVALID_HANDLE_VALUE)
	{
		printf("error: couldn't open iso, %d\n", GetLastError());
		return false;
	}

	ret = readVolumeHeader(&volumeHeader);

	if (!ret)
	{
		printf("mount(): could not read volume header from the filesystem\n");
		return false;
	}

	imageName = _strdup(path);

	if (!imageName)
	{
		printf("mount(): could not allocate memory for a copy of the image name\n");
		return false;
	}

	return true;
}

void FileSystem::unmount()
{
	if (fp != INVALID_HANDLE_VALUE)
		CloseHandle(fp);
	
	fp = INVALID_HANDLE_VALUE;
}

bool FileSystem::readVolumeHeader(VolumeHeader *vh)
{
	DWORD bytesRead;
	BOOL  ret;

	ret = ReadFile(
		fp,
		vh,
		volumeHeaderSize,
		&bytesRead,
		NULL);

	if (!ret)
	{
		printf("FileSystem::readVolumeHeader: couldn't read volume header from the iso, %d\n", GetLastError());
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
	DWORD bytesRead;
	BOOL  ret;

	ret = ReadFile(
		fp,
		dh,
		directoryHeaderSize,
		&bytesRead,
		NULL);

	if (!ret)
	{
		printf("error: couldn't directory header from the iso, %d\n", GetLastError());
		return false;
	}

	printf("\ngot %d bytes from the file\n\n", bytesRead);

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
		DWORD bytesRead;
		BOOL  ret;

		ret = ReadFile(
			fp,
			de,
			directoryEntrySize,
			&bytesRead,
			NULL);

		if (!ret)
		{
			printf("error: couldn't directory entry from the iso, %d\n", GetLastError());
			return false;
		}

		printf("\ngot %d bytes from the file\n\n", bytesRead);

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

			printf(
				"moving fp forward %d bytes (%d copies)\n", 
				pos,
				de->lastCopy);

			ret = seekToByte(pos, true);

			if (!ret)
				return false;
		}

		return true;
}

// TODO: do we care about the block boundaries here?
bool FileSystem::read(uint8_t *buf, const uint32_t bufLength, uint32_t *bytesRead)
{
	BOOL ret;

	if (!bufLength)
	{
		printf("read(): bufLength size of %d is invalid\n", bufLength);
		return false;
	}

	ret = ReadFile(
		fp,
		buf,
		bufLength,
		(DWORD *)bytesRead,
		NULL);

	if (!ret)
	{
		printf("FileSystem::read(): failed to read from filesystem: %d\n", GetLastError());
		return false;
	}

	printf("read(): read %d bytes from the file system\n", *bytesRead);

	return true;
}

bool FileSystem::seekToBlock(const uint32_t block, const bool relative)
{
	DWORD    fpPos;
	uint32_t pos = (volumeHeader.blockSize * block);

	fpPos = SetFilePointer(
		fp,
		pos,
		NULL,
		(relative ? FILE_CURRENT : FILE_BEGIN));

	if (fpPos == INVALID_SET_FILE_POINTER)
	{
		printf(
			"seekToBlock(): couldn't set file pointer position of %08x, %d\n", 
			pos,
			GetLastError());
		return false;
	}

	return true;
}

bool FileSystem::seekToByte(const uint32_t byte, const bool relative)
{
	DWORD    fpPos;
	
	fpPos = SetFilePointer(
		fp,
		byte,
		NULL,
		(relative ? FILE_CURRENT : FILE_BEGIN));

	if (fpPos == INVALID_SET_FILE_POINTER)
	{
		printf(
			"seekToBlock(): couldn't set file pointer position of %08x, %d\n", 
			byte,
			GetLastError());
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
	printf("recordType       = %02x\n", vh->recordType);
	printf("syncBytes        = ");
	for (int i = 0; i < 5; i++)
		printf("%02x", vh->syncBytes[i]);
	printf("\n");
	printf("recordVersion    = %02x\n", vh->recordVersion);
	printf("volumeFlags      = %02x\n", vh->flags);
	printf("volumeComment    = ");
	for (int i = 0; i < 32; i++)
		printf("%c", vh->comment[i]);
	printf("\n");
	printf("volumeLabel      = ");
	for (int i = 0; i < 32; i++)
		printf("%c", vh->label[i]);
	printf("\n");
	printf("volumeId         = %08x\n", vh->id);
	printf("blockSize        = %08x (%d bytes)\n", vh->blockSize, vh->blockSize);
	printf("blockCount       = %08x (%d KB in volume)\n", vh->blockCount, (vh->blockCount * vh->blockSize) / 1024);
	printf("rootDirId        = %08x\n", vh->rootDirId);
	printf("rootDirBlocks    = %08x (%d blocks)\n", vh->rootDirBlocks, vh->rootDirBlocks);
	printf("rootDirBlockSize = %08x (%d bytes)\n", vh->rootDirBlockSize, vh->rootDirBlockSize);
	printf("lastRootDirCopy  = %08x\n", vh->lastRootDirCopy);
	printf("rootDirCopies    = ");
	for (int i = 0; i < 8; i++)
	{
		printf("%08x", vh->rootDirCopies[i]);

		if (i + 1 < 8)
			printf(" ");
	}
	printf("\n");
}

void FileSystem::printDirectoryHeader(const DirectoryHeader *dh)
{
	printf("nextBlock       = %08x\n", dh->nextBlock);
	printf("prevBlock       = %08x\n", dh->prevBlock);
	printf("flags           = %08x\n", dh->flags);
	printf("unusedOffset    = %08x\n", dh->unusedOffset);
	printf("directoryOffset = %08x\n", dh->directoryOffset);
}

void FileSystem::printDirectoryEntry(const DirectoryEntry *de)
{
	printf("flags             = %08x\n", de->flags);
	printf("id                = %08x\n", de->id);
	printf("ext               = ");
	for (int i = 0; i < 4; i++)
		printf("%c", de->ext[i]);
	printf("\n");
	printf("blockSize         = %08x\n", de->blockSize);
	printf("entryLengthBytes  = %08x\n", de->entryLengthBytes);
	printf("entryLengthBlocks = %08x\n", de->entryLengthBlocks);
	printf("burst             = %08x\n", de->burst);
	printf("gap               = %08x\n", de->gap);
	printf("fileName          = ");
	for (int i = 0; i < 32; i++)
		printf("%c", de->fileName[i]);
	printf("\n");
	printf("lastCopy          = %08x\n", de->lastCopy);
	printf("copies            = %08x\n", de->copies);
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
			printf("\n");

		printf("%c", str[i]);
	}

	printf("\n");
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
