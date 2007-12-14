#ifndef _INC_CONSOLE
#define _INC_CONSOLE

#include "types.h"
#include "ARM60CPU.h"
#include "DMA.h"

//////////////////////////////////////////////////

class Console
{
public:
   Console ();
   ~Console ();

private:
   DMA*      m_DMA;
   ARM60CPU* m_CPU;
};

#endif //_INC_CONSOLE
