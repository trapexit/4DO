#ifndef _INC_DMA
#define _INC_DMA

#include "types.h"

//////////////////////////////////////////////////

#define DRAM_SIZE 0x200000 // 2 megs of DRAM

class DMAController
{
public:
   DMAController ();
   ~DMAController ();

   uint GetValue (uint address);
   void SetValue (uint address, uint value);
   uchar* GetDRAMPointer (uint address);

private:
   uchar* m_DRAM;
};

#endif //_INC_DMA