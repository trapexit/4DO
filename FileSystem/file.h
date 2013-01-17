#ifndef _FILE_H_
#define _FILE_H_

#include "directory.h"

class File
{
	public:
		File(const char *rom);
		~File();

		// 
		// openFile:
		//     opens a file on a 3do disc, and prepares to read
		//     it's contents
		// 
		// arguments:
		//     1) const char *path (IN): the 3do filesystem path 
		//         to the file to be opened
		// 
		// return value:
		//     true on success, false otherwise
		// 
		bool openFile(const char *path);

		// 
		// closeFile:
		//     closes the currently opened file, if there is one
		// 
		// arguments:
		//     none
		// 
		// return value:
		//     true on success, false otherwise
		// 
		bool closeFile();

		// 
		// read:
		//     reads an arbitrary number of bytes from the currently
		//     opened file.
		// 
		// arguments:
		//     1) uint8_t *buf (OUT): a buffer that will be filled with
		//         the data read from teh file
		//     2) const uint32_t bufLength (IN): the length of the buffer
		//         in parameter 1.  also constitutes the maximum number
		//         of bytes that will be read from the file.
		//     3) uint32_t *bytesRead (OUT): the number of bytes that 
		//         were actually read from the file.
		// 
		// return value:
		//     true on success, false otherwise
		// 
		bool read(uint8_t *buf, const uint32_t bufLength, uint32_t *bytesRead);

		bool readLine(uint8_t *buf, const uint32_t bufLength, uint32_t *bytesRead);

		// 
		// getFileSize: 
		//     returns the file size of the currently opened file
		// 
		// arguments:
		//     none
		// 
		// return value:
		//     the number of bytes in the currently opened file.  
		//     will return 0 if there is no file currently open
		// 
		uint32_t getFileSize();

		// 
		// getFileType:
		//     returns the type of the currently opened file
		// 
		// arguments:
		//     none
		// 
		// return value:
		//     a uint32_t value representing the type of the file
		//     possibilities are DirectoryEntryTypeFile and DirectoryEntryTypeFolder
		//     will return 0 on failure (file not open etc)
		// 
		uint32_t getFileType();

		// 
		// getFileExt:
		//     returns the file extension, if any, for the currently opened file
		// 
		// arguments:
		//     none
		// 
		// return value:
		//     a constant pointer to a uint8_t buffer 4 bytes in length.
		// 
		const uint8_t *getFileExt();

		// 
		// getFileName:
		//     returns the file name of the currently opened file
		// 
		// arguments:
		//     none
		// 
		// return value:
		//     a constant pointer to a char array containing the file name
		// 
		const char *getFileName();
	
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
