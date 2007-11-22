#include <math.h>
#include "BitMath.h"

uint SetBit (uint value, int bitNumber, bool bitValue)
{
   return SetBits (value, ((uint) pow (2, bitNumber)), 0xFFFFFFFF);
}
   
uint SetBits (uint value, uint setMask, uint setValue)
{
   return (value & ~setMask) + (setMask & setValue);
}