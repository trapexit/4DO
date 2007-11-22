#ifndef _INC_BITMATH
#define _INC_BITMATH

#include "types.h"

//////////////////////////////////////////////////

uint SetBit (uint value, int bitNumber, bool bitValue);
uint SetBits (uint value, uint setMask, uint setValue);

char* CharToBitString (char value);
char* UintToBitString (uint value);

#endif //_INC_BITMATH