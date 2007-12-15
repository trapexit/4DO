#include "BitMath.h"

uint SetBit (uint value, int bitNumber, bool bitValue)
{
   return SetBits (value, ((uint) pow ((double) 2, bitNumber)), 0xFFFFFFFF);
}
   
uint SetBits (uint value, uint setMask, uint setValue)
{
   return (value & ~setMask) + (setMask & setValue);
}

wxString CharToBitString (char value)
{
   const int  bits = 8;
   char*      retVal;
   wxString   newRetVal;

   retVal = new char[bits + 1];
   for (int x = 0; x < bits; x++)
   {
      sprintf (&retVal [x], "%d", ((value & ((uint) pow ((double) 2, bits - x - 1))) > 0) ? 1 : 0);
   }
   retVal [bits] = 0;
   
   newRetVal = _T (retVal);
   delete retVal;
   
   return newRetVal;
}

wxString UintToBitString (uint value)
{
   const int  bits = 32;
   char*      retVal;
   wxString   newRetVal;

   retVal = new char[bits + 1];
   for (int x = 0; x < bits; x++)
   {
      sprintf (&retVal [x], "%d", ((value & ((uint) pow ((double) 2, bits - x - 1))) > 0) ? 1 : 0);
   }
   retVal [bits] = 0;
   
   newRetVal = _T (retVal);
   delete retVal;

   return newRetVal;
}