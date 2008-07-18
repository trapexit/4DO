#ifndef _INC_DMA
#define _INC_DMA

#include "types.h"

//////////////////////////////////////////////////

#define DRAM_SIZE 0x200000 // 2 megs of DRAM
#define VRAM_SIZE 0x100000 // 1 megs of VRAM
#define BIOS_SIZE 0x100000 // 1 meg  of BIOS
#define VRAM_SIZE 0x100000 // 1 meg  of BIOS

class DMAController
{
public:
	DMAController ();
	~DMAController();

	uchar GetByte( uint address );
	void  SetByte( uint address, uchar value );

	uint  GetWord( uint address );
	void  SetWord( uint address, uint  value );
	uchar* GetRAMPointer( uint address );

private:
	uchar* m_DRAM;
	uchar* m_VRAM;
	uchar* m_BIOS;
};

#endif //_INC_DMA
