#ifndef _DIRECTORY_H_
#define _DIRECTORY_H_

#include "filesystem.h"

class Directory
{
	public:
		Directory(const char *rom);
		~Directory();

		bool openDirectory(const char *path);
		bool closeDirectory();

		bool changeDirectory(const char *path);
		bool getDirectory(const char *path);

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
