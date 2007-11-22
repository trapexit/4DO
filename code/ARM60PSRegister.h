#ifndef _INC_ARM60PSREGISTER
#define _INC_ARM60PSREGISTER

#include "ARM60Register.h"

enum CPUMode
{
   CPUMODE_USR = 0x10, // Normal program execution state.
   CPUMODE_FIQ = 0x11, // Designed to support a data transfer or channel process
   CPUMODE_IRQ = 0x12, // Used for general purpose interrupt handling
   CPUMODE_SVC = 0x13, // A protected mode for the operating system.
   CPUMODE_ABT = 0x17, // Entered after a data or instruction prefetch abort.
   CPUMODE_UND = 0x1B, // Entered when an undefined instruction is executed.
   CPUMODE_INVALID = 0 // NOTE: I'm unsure if this will ever happen, but I don't want it to crash on invalid data.
};

class ARM60PSRegister : public ARM60Register
{
public:
   ARM60PSRegister::ARM60PSRegister (uint* value);
   
   bool GetNegative ();
   bool GetZero ();
   bool GetCarry ();
   bool GetOverflow ();
   bool GetFIQDisable ();
   bool GetIRQDisable ();
   CPUMode GetCPUMode ();

   void SetNegative (bool value);
   void SetZero (bool value);
   void SetCarry (bool value);
   void SetOverflow (bool value);
   void SetFIQDisable (bool value);
   void SetIRQDisable (bool value);
   void SetCPUMode (CPUMode value);
};

#endif //_INC_ARM60PSREGISTER