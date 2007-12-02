#include "BitMath.h"

uint SetBit (uint value, int bitNumber, bool bitValue)
{
   return SetBits (value, ((uint) pow (2, bitNumber)), 0xFFFFFFFF);
}
   
uint SetBits (uint value, uint setMask, uint setValue)
{
   return (value & ~setMask) + (setMask & setValue);
}

char* CharToBitString (char value)
{
   const bits = 8;
   char* retVal;

   retVal = new char[bits + 1];
   for (int x = 0; x < bits; x++)
   {
      sprintf (&retVal [x], "%d", ((value & ((uint) pow (2, bits - x - 1))) > 0) ? 1 : 0);
   }
   retVal [bits] = 0;

   return retVal;
}

char* UintToBitString (uint value)
{
   const bits = 32;
   char* retVal;

   retVal = new char[bits + 1];
   for (int x = 0; x < bits; x++)
   {
      sprintf (&retVal [x], "%d", ((value & ((uint) pow (2, bits - x - 1))) > 0) ? 1 : 0);
   }
   retVal [bits] = 0;

   return retVal;
}