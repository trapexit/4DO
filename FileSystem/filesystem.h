// 
// TODO: put this in a namespace?
// TODO: throw the current directory in a member so we can 
//       know the block size we're dealing with.  this also
//       makes reading directories easier, as we can read them
//       a block at a time since we know the block size
// 

#ifndef _FILESYSTEM_H_
#define _FILESYSTEM_H_

#include "wx/file.h"

#ifndef GENRE_UNIX
typedef unsigned char  uint8_t;
typedef unsigned short uint16_t;
typedef unsigned int   uint32_t;
typedef __int32        int32_t;
#endif

// 
// filesystem object sizes
// 
const uint8_t volumeHeaderSize    = 132;
const uint8_t directoryHeaderSize = 20;
const uint8_t directoryEntrySize  = 72;

// 
// directory header constants
// 
const uint32_t DirectoryHeaderLastBlock  = 0xFFFFFFFF;
const uint32_t DirectoryHeaderFirstBlock = 0xFFFFFFFF;

// 
// directory entry flags
// 
const uint32_t DirectoryEntryTypeFile         = 0x00000002;
const uint32_t DirectoryEntryTypeSpecial      = 0x00000006;
const uint32_t DirectoryEntryTypeFolder       = 0x00000007;
const uint32_t DirectoryEntryTypeMask         = 0x0000000F;
const uint32_t DirectoryEntryPosLastInBlock   = 0x40000000;
const uint32_t DirectoryEntryPosLastInDir     = 0xC0000000;
const uint32_t DirectoryEntryPosMask          = 0xF0000000;

struct VolumeHeader             // 132 bytes
{
	uint8_t  recordType;        // 1 byte
	uint8_t  syncBytes[5];      // 5 bytes
	uint8_t  recordVersion;     // 1 byte
	uint8_t  flags;             // 1 byte
	uint8_t  comment[32];       // 32 bytes
	uint8_t  label[32];         // 32 bytes
	uint32_t id;                // 4 bytes
	uint32_t blockSize;         // 4 bytes
	uint32_t blockCount;        // 4 bytes
	uint32_t rootDirId;         // 4 bytes
	uint32_t rootDirBlocks;     // 4 bytes
	uint32_t rootDirBlockSize;  // 4 bytes
	uint32_t lastRootDirCopy;   // 4 bytes
	uint32_t rootDirCopies[8];  // 32 bytes
};

struct DirectoryHeader        // 20 bytes
{
	int32_t  nextBlock;       // 4 bytes
	int32_t  prevBlock;       // 4 bytes
	uint32_t flags;           // 4 bytes
	uint32_t unusedOffset;    // 4 bytes
	uint32_t directoryOffset; // 4 bytes
};

struct DirectoryEntry           // 72 bytes
{
	uint32_t flags;             // 4 bytes
	uint32_t id;                // 4 bytes
	uint8_t  ext[4];            // 4 bytes
	uint32_t blockSize;         // 4 bytes
	uint32_t entryLengthBytes;  // 4 bytes
	uint32_t entryLengthBlocks; // 4 bytes
	uint32_t burst;             // 4 bytes
	uint32_t gap;               // 4 bytes
	uint8_t  fileName[32];      // 32 bytes
	uint32_t lastCopy;          // 4 bytes

	// 
	// note that this field is actually of variable length.
	// specifically, (lastCopy + 1) * 4 bytes.  we don't really
	// care about any other copies though, we only really need 
	// one copy.
	// 
	uint32_t copies;            // 4 bytes
};

class FileSystem
{
	public:
		FileSystem();
		~FileSystem();

		// 
		// mount: 
		//     will open a handle to the file at path, read the 
		//     volume header, and position the file handle at the 
		//     root directory
		// 
		// arguments:
		//     1) const char *path (IN): the filesystem path where a 3do iso resides
		// 
		// return value:
		//     true on success, false otherwise
		// 
		bool mount(const char *path);

		// 
		// unmount:
		//     closes the file handle to the 3do iso opened with mount
		// 
		// arguments:
		//     none
		// 
		// return value:
		//     none
		// 
		void unmount();

		// 
		// filesystem read operations
		// 

		// 
		// readVolumeHeader: 
		//     reads the filesystem volume header.  this header should
		//     be at the beginning of every 3do iso/disc.  it contains 
		//     information about the filesystem on the disc, including
		//     block size, total disc size, root directory location, etc.
		// 
		// arguments:
		//     1) VolumeHeader *vh (OUT): a pointer to an allocated VolumeHeader object.
		//         this will be initialized with the data read from the disc
		// return value:
		//     true on success, false otherwise
		// 
		bool readVolumeHeader(VolumeHeader *vh);

		// 
		// readDirectoryHeader: 
		//     reads a directory header from the filesystem.  a directory
		//     header contains information about a directory.  mostly this 
		//     is just information about the next and previous blocks in this
		//     directory
		// 
		// arguments:
		//     1) DirectoryHeader *de (OUT): a pointer to an allocated DirectoryHeader
		//         object.  this will be initialized with the data read from the disc
		// 
		// return value:
		//     true on success, false otherwise
		// 
		bool readDirectoryHeader(DirectoryHeader *dh);

		// 
		// readDirectoryEntry: 
		//     reads a directory entry from the filesystem, initializing
		//     the passed in object with the data read.  a directory entry
		//     is basically a pointer to some object residing in a directory.
		//     this object can be either another directory, or a file.
		// 
		// arguments:
		//     1) DirectoryEntry *de (OUT): a pointer to an allocated DirectoryEntry 
		//         object that will be initialized with the data read from the disc
		// 
		// return value:
		//     true on success, false otherwise
		// 
		bool readDirectoryEntry(DirectoryEntry *de);

		// 
		// read:
		//     reads the specified number of bytes from the current position
		//     in the filesystem
		// 
		// arguments:
		//     1) uin8_t *buf (OUT): will be filled with the bytes read
		//     2) const uint32_t bufLength (IN): the length of first parameter,
		//         and the maximum number of bytes that will be read
		//     3) uint32_t *bytesRead (OUT): the number of bytes actually read
		// 
		// return value:
		//     true on success, false otherwise
		// 
		bool read(void *buf, const uint32_t bufLength, uint32_t *bytesRead);

		// 
		// seekToBlock: 
		//    seeks the current read position to the block specified
		// 
		// arguments:
		//     1) const uint32_t block (IN): the block to move the filesystem position to
		//     2) const bool relative (IN): determins whether the seeking should be
		//         done relative to the current position or from the beginning of the file
		// 
		// return value:
		//     true on success, false otherwise
		// 
		bool seekToBlock(const uint32_t block, bool relative);

		// 
		// seekToByte:
		//    seeks to the byte specified in the filesystem
		// 
		// arguments:
		//     1) const uint32_t byte (IN): the byte to move to
		//     2) const bool relative (IN): determines whether the seeking should be 
		//         done relative to the current position or from the beginning of the file
		// 
		// return value:
		//     true on success, false otherwise
		// 
		bool seekToByte(const uint32_t byte, bool relative);

		// 
		// getBlockSize:
		//     currently returns the block size of the mounted volume.  not sure
		//     if this should be changed to return the block size of the current 
		//     filesystem entry, as it seems that in theory the block size could 
		//     vary by directory.  in practice the block size remains the same so far
		// 
		// arguments:
		//     none
		// 
		// return value:
		//     the block size of the currently mounted volume
		// 
		uint32_t getBlockSize();

		// 
		// getImageName:
		//     gets the image name of the currently mounted file
		// 
		// arguments:
		//     none
		// 
		// return value:
		//     const char * containing the image name on disk
		// 
		const char *getImageName();

		// 
		// logging operations
		// 

		static void printVolumeHeader(const VolumeHeader *vh);
		static void printDirectoryHeader(const DirectoryHeader *dh);
		static void printDirectoryEntry(const DirectoryEntry *de);

		static void printString(const char *str);
		static void printString(const uint8_t *str, const uint32_t strLength);

	private:
		// 
		// endian swappers
		// 

		void endianSwap(uint16_t &x);
		void endianSwap(uint32_t &x);
		void endianSwap(int32_t &x);

		// file handle to the iso/rom we're mounting
		wxFile file;

		VolumeHeader volumeHeader;

		// 
		// the block size of the volume
		// 
		uint32_t blockSize;

		// 
		// the path on disk to the image we loaded the filesystem from
		// 
		const char *imageName;
};

#endif // #ifndef _FILESYSTEM_H_
