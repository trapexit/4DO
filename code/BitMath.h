#ifndef _INC_BITMATH
#define _INC_BITMATH

#include "types.h"

//////////////////////////////////////////////////

uint SetBit (uint value, int bitNumber, bool bitValue);
uint SetBits (uint value, uint setMask, uint setValue);

#endif //_INC_BITMATH