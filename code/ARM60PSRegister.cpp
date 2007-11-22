#include "ARM60PSRegister.h"

ARM60PSRegister::ARM60PSRegister (uint* value) : ARM60Register(value) {}

bool ARM60PSRegister::GetNegative ()
{
   return *value & 0x80000000;
}

bool ARM60PSRegister::GetZero ()
{
   return *value & 0x40000000;
}

bool ARM60PSRegister::GetCarry ()
{
   return *value & 0x20000000;
}

bool ARM60PSRegister::GetOverflow ()
{
   return *value & 0x10000000;
}

bool ARM60PSRegister::GetFIQDisable ()
{
   return *value & 0x00000080;
}

bool ARM60PSRegister::GetIRQDisable ()
{
   return *value & 0x00000040;
}