#include <iostream.h>
#include "types.h"
#include "ARM60Registers.h"

ARM60Registers::ARM60Registers ()
{
   m_regs = new uint [REG_COUNT];

   // Potentially pointless initialization.
   for (int x = 0; x < REG_COUNT; x++)
   {
      m_regs [x] = 0;
   }
}

ARM60Registers::~ARM60Registers ()
{
   delete m_regs;
}