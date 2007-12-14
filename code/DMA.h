#ifndef _INC_DMA
#define _INC_DMA

#include "types.h"

//////////////////////////////////////////////////

class DMA
{
public:
   DMA ();
   ~DMA ();

   uint GetValue (uint address);
   void SetValue (uint address, uint value);

private:
};

#endif //_INC_DMA
