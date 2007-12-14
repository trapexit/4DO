#ifndef _INC_CONSOLE
#define _INC_CONSOLE

#include "types.h"
#include "ARM60CPU.h"
#include "DMAController.h"

//////////////////////////////////////////////////

class Console
{
public:
   Console ();
   ~Console ();

private:
   DMAController*  m_DMA;
   ARM60CPU*       m_CPU;
};

#endif //_INC_CONSOLE

