#include "DMAController.h"

// I found this out of the MESS project's 3do file. 
// It matches mappings found in the FreeDO project.
/*
AM_RANGE(0x00000000, 0x001FFFFF) AM_RAMBANK(1) AM_BASE(&dram)              // DRAM
AM_RANGE(0x00200000, 0x002FFFFF) AM_RAM	AM_BASE(&vram)                     // VRAM
AM_RANGE(0x03000000, 0x030FFFFF) AM_ROMBANK(2)                             // BIOS
AM_RANGE(0x03140000, 0x0315FFFF) AM_READWRITE(nvarea_r, nvarea_w)          // NVRAM
AM_RANGE(0x03180000, 0x031FFFFF) AM_READWRITE(unk_318_r, unk_318_w)        // ????
AM_RANGE(0x03200000, 0x032FFFFF) AM_READWRITE(vram_sport_r, vram_sport_w)  // special vram access1
AM_RANGE(0x03300000, 0x033FFFFF) AM_READWRITE(madam_r, madam_w)            // address decoder
AM_RANGE(0x03400000, 0x034FFFFF) AM_READWRITE(clio_r, clio_w)              // io controller
*/

DMAController::DMAController()
{
	// Constructor
	
	m_DRAM = new uchar[ DRAM_SIZE ];
	m_VRAM = new uchar[ VRAM_SIZE ];
	m_BIOS = new uchar[ BIOS_SIZE ];
	
	// Potentially pointless initialization.
	for (int x = 0; x < DRAM_SIZE; x++)
	{
		m_DRAM[ x ] = 0;
	}

	// Potentially pointless initialization.
	for (int x = 0; x < VRAM_SIZE; x++)
	{
		m_VRAM[ x ] = 0;
	}

	// Potentially pointless initialization.
	for (int x = 0; x < BIOS_SIZE; x++)
	{
		m_BIOS[ x ] = 0;
	}
}

DMAController::~DMAController()
{
	// Destructor
	delete m_DRAM;
	delete m_VRAM;
	delete m_BIOS;
}

uchar DMAController::GetByte(uint address)
{
	if   ( ( address & 0xFFE00000) == 0x00000000 )
	{
		// DRAM
		return m_DRAM[ address ];
	}
	else if( address >= 0x00200000 && address <= 0x002FFFFF )
	{
		// VRAM
		address -= 0x00200000;
		return m_VRAM[ address ];
	}
	else if( address >= 0x03000000 && address <= 0x030FFFFF )
	{
		// BIOS
		address -= 0x03000000;
		return m_BIOS[ address ];
	}
	else if( address >= 0x03140000 && address <= 0x0315FFFF )
	{
		// NVRAM
	}
	else if( address >= 0x03180000 && address <= 0x031FFFFF )
	{
		// ????
	}
	else if( address >= 0x03200000 && address <= 0x032FFFFF )
	{
		// Special VRAM Access1
	}
	else if( address >= 0x03300000 && address <= 0x033FFFFF )
	{
		// Address Decoder
	}
	else if( address >= 0x03400000 && address <= 0x034FFFFF )
	{
		// IO Controller
	}
	
	return 0;
}

uint DMAController::GetWord( uint address )
{
	if   ( ( address & 0xFFE00000) == 0x00000000 )
	{
		// DRAM
		address -= address % 4;
		return ( m_DRAM[ address ] << 24 ) + ( m_DRAM[ address + 1 ] << 16 ) + ( m_DRAM[ address + 2 ] << 8 ) + m_DRAM[ address + 3 ];
	}
	else if( address >= 0x00200000 && address <= 0x002FFFFF )
	{
		// VRAM
		address -= 0x00200000;
		address -= address % 4;
		return ( m_VRAM[ address ] << 24 ) + ( m_VRAM[ address + 1 ] << 16 ) + ( m_VRAM[ address + 2 ] << 8 ) + m_VRAM[ address + 3 ];
	}
	else if( address >= 0x03000000 && address <= 0x030FFFFF )
	{
		// BIOS
		address -= 0x03000000;
		address -= address % 4;
		return ( m_BIOS[ address ] << 24 ) + ( m_BIOS[ address + 1 ] << 16 ) + ( m_BIOS[ address + 2 ] << 8 ) + m_BIOS[ address + 3 ];
	}
	else if( address >= 0x03140000 && address <= 0x0315FFFF )
	{
		// NVRAM
	}
	else if( address >= 0x03180000 && address <= 0x031FFFFF )
	{
		// ????
	}
	else if( address >= 0x03200000 && address <= 0x032FFFFF )
	{
		// Special VRAM Access1
	}
	else if( address >= 0x03300000 && address <= 0x033FFFFF )
	{
		// Address Decoder
	}
	else if( address >= 0x03400000 && address <= 0x034FFFFF )
	{
		// IO Controller
	}
	
	return 0;
}

void DMAController::SetByte( uint address, uchar value )
{
	if   ( ( address & 0xFFE00000 ) == 0x00000000 )
	{
		// DRAM
		m_DRAM[ address ] = value;
	}
	else if( address >= 0x00200000 && address <= 0x002FFFFF )
	{
		// VRAM
		address -= 0x00200000;
		m_VRAM[ address ] = value;
	}
	else if( address >= 0x03000000 && address <= 0x030FFFFF )
	{
		// BIOS
		address -= 0x03000000;
		m_BIOS[ address ] = value;
	}
	else if( address >= 0x03140000 && address <= 0x0315FFFF )
	{
		// NVRAM
	}
	else if( address >= 0x03180000 && address <= 0x031FFFFF )
	{
		// ????
	}
	else if( address >= 0x03200000 && address <= 0x032FFFFF )
	{
		// Special VRAM Access1
	}
	else if( address >= 0x03300000 && address <= 0x033FFFFF )
	{
		// Address Decoder
	}
	else if( address >= 0x03400000 && address <= 0x034FFFFF )
	{
		// IO Controller
	}
}

void DMAController::SetWord( uint address, uint value )
{
	if    ( ( address & 0xFFE00000 ) == 0x00000000 )
	{
		// DRAM
		address -= address % 4;
		m_DRAM[ address     ] = ( uchar )( ( value & 0xFF000000 ) >> 24 );
		m_DRAM[ address + 1 ] = ( uchar )( ( value & 0x00FF0000 ) >> 16 );
		m_DRAM[ address + 2 ] = ( uchar )( ( value & 0x0000FF00 ) >> 8  );
		m_DRAM[ address + 3 ] = ( uchar )(   value & 0x000000FF );
	}
	else if( address >= 0x00200000 && address <= 0x002FFFFF )
	{
		// VRAM
		address -= 0x00200000;
		address -= address % 4;
		m_VRAM[ address     ] = ( uchar )( ( value & 0xFF000000 ) >> 24 );
		m_VRAM[ address + 1 ] = ( uchar )( ( value & 0x00FF0000 ) >> 16 );
		m_VRAM[ address + 2 ] = ( uchar )( ( value & 0x0000FF00 ) >> 8  );
		m_VRAM[ address + 3 ] = ( uchar )(   value & 0x000000FF );
	}
	else if( address >= 0x03000000 && address <= 0x030FFFFF )
	{
		// BIOS
		address -= 0x03000000;
		address -= address % 4;
		m_BIOS[ address     ] = ( uchar )( ( value & 0xFF000000 ) >> 24 );
		m_BIOS[ address + 1 ] = ( uchar )( ( value & 0x00FF0000 ) >> 16 );
		m_BIOS[ address + 2 ] = ( uchar )( ( value & 0x0000FF00 ) >> 8  );
		m_BIOS[ address + 3 ] = ( uchar )(   value & 0x000000FF );
	}
	else if( address >= 0x03140000 && address <= 0x0315FFFF )
	{
		// NVRAM
	}
	else if( address >= 0x03180000 && address <= 0x031FFFFF )
	{
		// ????
	}
	else if( address >= 0x03200000 && address <= 0x032FFFFF )
	{
		// Special VRAM Access1
	}
	else if( address >= 0x03300000 && address <= 0x033FFFFF )
	{
		// Address Decoder
	}
	else if( address >= 0x03400000 && address <= 0x034FFFFF )
	{
		// IO Controller
	}
}

uchar* DMAController::GetRAMPointer( uint address )
{
	if( address >= 0x03000000 && address <= 0x030FFFFF )
	{
		// BIOS
		return &( m_BIOS[ address - 0x03000000 ] );
	}
	else if( address >= 0x00200000 && address <= 0x002FFFFF )
	{
		// VRAM
		return &( m_VRAM[ address - 0x00200000 ] );
	}
	else
	{
		// just use DRAM
		return &( m_DRAM[ address ] );
	}
}
