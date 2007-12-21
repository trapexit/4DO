#ifndef _FILE_H_
#define _FILE_H_

#include "directory.h"

class File
{
	public:
		File(const char *rom);
		~File();

		bool openFile(const char *path);
		bool closeFile();

		bool read(uint8_t *buf, const uint32_t bufLength, uint32_t *bytesRead);
		bool readLine(uint8_t *buf, const uint32_t bufLength, uint32_t *bytesRead);

		uint32_t getFileSize();
	
	private:
		FileSystem fileSystem;

		// 
		// the directory entry for the opened file
		// 
		DirectoryEntry dirEntry;

		// 
		// end of file indicator
		// 
		bool endOfFile;

		// 
		// currently read bytes
		// 
		uint32_t currBytes;
};

#endif // #ifndef _FILE_H_
