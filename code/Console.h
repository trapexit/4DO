#ifndef _INC_CONSOLE
#define _INC_CONSOLE

#include "types.h"
#include "ARMCPU.h"
#include "DMAController.h"

//////////////////////////////////////////////////

class Console
{
public:
   Console  ();
   ~Console ();
   
   ARMCPU*         CPU ();
   DMAController*  DMA ();

private:
   DMAController*  m_DMA;
   ARMCPU*         m_CPU;
};

#endif //_INC_CONSOLE