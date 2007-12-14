#ifndef _INC_DMA
#define _INC_DMA

#include "types.h"

//////////////////////////////////////////////////

class DMAController
{
public:
   DMAController ();
   ~DMAController ();

   uint GetValue (uint address);
   void SetValue (uint address, uint value);

private:
};

#endif //_INC_DMA

