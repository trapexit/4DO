#ifndef _DIRECTORY_H_
#define _DIRECTORY_H_

#include "filesystem.h"

class Directory
{
	public:
		Directory(const char *rom);
		~Directory();

		// 
		// openDirectory: 
		//     opens a directory at the path specified
		//     if this call succeeds, you will be ready to 
		//     enumerate directory contents
		// 
		// arguments:
		//     1) const char *path (IN): the filesystem path
		//         to the directory to open
		// 
		// return value:
		//     true on success, false otherwise
		// 
		bool openDirectory(const char *path);

		// 
		// closeDirectory: 
		//     closes a directory the open directory, if any
		// 
		// arguments:
		//     none
		// 
		// return value:
		//     true on success, false otherwise
		// 
		bool closeDirectory();

		// 
		// changeDirectory:
		//     changes the currently open directory to the one
		//     specified in the path
		// 
		// arguments:
		//     1) const char *path (IN): the filesystem path
		//         to the directory to be changed to
		// 
		// return value:
		//     true on success, false otherwise
		// 
		bool changeDirectory(const char *path);

		bool getDirectory(const char *path);

		// 
		// enumerateDirectory:
		//     enumerates the contents of the currently opened
		//     directory.  the details of the DirectoryEntry param
		//     can be found in filesystem.h
		// 
		// arguments:
		//     1) DirectoryEntry *de (OUT): will contain the information
		//         about the next directory entry in the currently opened
		//         directory after completion
		// 
		// return value:
		//     true on success, false otherwise
		// 
		bool enumerateDirectory(DirectoryEntry *de);
	private:
		// 
		// the file system.
		// 
		FileSystem fileSystem;

		// 
		// this will be initialized to the directory found 
		// after openDirectory was called.  basically the current directory
		// 
		DirectoryHeader directoryHeader;

		// 
		// used in enumerateDirectory so we know when we've seen the last
		// directory entry in the directory.
		// 
		bool endOfDir;
};

#endif // #ifndef _DIRECTORY_H_
