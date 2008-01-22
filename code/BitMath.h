#ifndef _INC_BITMATH
#define _INC_BITMATH

#include "types.h"
#include "wx/wx.h"

#include <math.h>
#include <cstdio>

//////////////////////////////////////////////////

uint SetBit (uint value, int bitNumber, bool bitValue);
uint SetBits (uint value, uint setMask, uint setValue);

wxString CharToBitString (char value);
wxString UintToBitString (uint value);
wxString UintToHexString (uint value);

#endif //_INC_BITMATH