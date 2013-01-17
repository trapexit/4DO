#ifndef _DIRECTORY_H_
#define _DIRECTORY_H_

#include "filesystem.h"
#include <list>

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

		// 
		// getPath:
		//     returns the current filesystem path
		// 
		// arguments:
		//     none
		// 
		// return value:
		//     constant character pointer to a c string containing 
		//     the current filesystem path of the directory object
		// 
		const char *getPath();

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

		// 
		// findInCurrentDirectory:
		//     finds a folder of the given name in the currently 
		//     opened folder.  if the item is found, this function will
		//     move the filesystem fp to the beginning of the directory
		//     and read the directory header.  also the directory tree and
		//     current filesystem path will be updated
		// 
		// arguments:
		//     1) const char *item (IN): the string to search for
		//     2) DirectoryEntry *dirEntry (OUT): will contain the directory
		//         information of the folder requested, if found.
		// 
		// return value:
		//     true if the folder is found, false otherwise
		// 
		bool findInCurrentDirectory(const char *item, DirectoryEntry *dirEntry);
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
		// the directory hierarchy.  used for searching based on relative paths
		// 
		std::list<DirectoryEntry *> dirTree;

		// 
		// our current directory path
		// 
		wxString path;

		// 
		// used in enumerateDirectory so we know when we've seen the last
		// directory entry in the directory.
		// 
		bool endOfDir;
};

#endif // #ifndef _DIRECTORY_H_
