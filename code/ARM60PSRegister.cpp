#include "ARM60PSRegister.h"

ARM60PSRegister::ARM60PSRegister (uint* value) : ARM60Register(value) 
{
   // Nothin special.
}

bool ARM60PSRegister::GetNegative ()
{
   return (*Value & 0x80000000) > 0;
}

void ARM60PSRegister::SetNegative (bool value)
{
   *Value = SetBits (*Value, 0x80000000, value ? 0xFFFFFFFF : 0);
}

bool ARM60PSRegister::GetZero ()
{
   return (*Value & 0x40000000) > 0;
}

void ARM60PSRegister::SetZero (bool value)
{
   *Value = SetBits (*Value, 0x40000000, value ? 0xFFFFFFFF : 0);
}

bool ARM60PSRegister::GetCarry ()
{
   return (*Value & 0x20000000) > 0;
}

void ARM60PSRegister::SetCarry (bool value)
{
   *Value = SetBits (*Value, 0x20000000, value ? 0xFFFFFFFF : 0);
}

bool ARM60PSRegister::GetOverflow ()
{
   return (*Value & 0x10000000) > 0;
}

void ARM60PSRegister::SetOverflow (bool value)
{
   *Value = SetBits (*Value, 0x10000000, value ? 0xFFFFFFFF : 0);
}

bool ARM60PSRegister::GetFIQDisable ()
{
   return (*Value & 0x00000080) > 0;
}

void ARM60PSRegister::SetFIQDisable (bool value)
{
   *Value = SetBits (*Value, 0x00000080, value ? 0xFFFFFFFF : 0);
}

bool ARM60PSRegister::GetIRQDisable ()
{
   return (*Value & 0x00000040) > 0;
}

void ARM60PSRegister::SetIRQDisable (bool value)
{
   *Value = SetBits (*Value, 0x00000040, value ? 0xFFFFFFFF : 0);
}

CPUMode ARM60PSRegister::GetCPUMode ()
{
   uint mode;
   mode = (*Value & 0x1F);
   
   switch (mode)
   {
   case 0x10:
      return CPUMODE_USR;
   
   case 0x11:
      return CPUMODE_FIQ;
   
   case 0x12:
      return CPUMODE_IRQ;
   
   case 0x13:
      return CPUMODE_SVC;
   
   case 0x17:
      return CPUMODE_ABT;
   
   case 0x1B:
      return CPUMODE_UND;
   
   }
   return CPUMODE_INVALID;
}


void ARM60PSRegister::SetCPUMode (CPUMode value)
{
   *Value = SetBits (*Value, 0x0000001F, value);
}