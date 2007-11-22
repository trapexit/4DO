#include <iostream.h>
#include "ARM60CPU.h"
#include "ARM60Registers.h"

void main()
{
   ARM60Registers* regs;

   for (int x = 0; x < 1; x++)
   {
      regs = new ARM60Registers ();
      delete regs;
   }
}