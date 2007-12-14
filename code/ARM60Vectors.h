#ifndef _INC_ARM60VECTORS
#define _INC_ARM60VECTORS

#include "types.h"

#define VECT_COUNT 8

/*
NOTES for later:

When multiple exceptions arise at the same time, a fixed priority system 
determines the order in which they will be handled:
   
   (highest priority)
   1. VECTOR_RESET 
   2. VECTOR_ABORT_DATA
   3. VECTOR_FIQ
   4. VECTOR_IRQ
   5. VECTOR_ABORT_PREFETCH
   6. VECTOR_UNDEFINED + VECTOR_SOFTWARE
   (lowest priority)
   
   If a data abort occurs at the same time as a FIQ, and FIQ's are enabled
   (F in CPSR is 0), ARM60 will enter the data abort handler and then
   immediately proceed to the FIQ vector. A normal return from FIQ will
   cause the data abort handler to resume execution. Placing data abort
   higher than FIQ is necessary to ensure that the transfer error does
   not escape detection. The time for this exception entry should be
   added to worst case FIQ latency calculations.
*/

enum VectorType
{
   VECTOR_RESET          = 0x00000000,  // Supervisor mode
   VECTOR_UNDEFINED      = 0x00000004,  // Undefined mode
   VECTOR_SOFTWARE       = 0x00000008,  // Supervisor mode
   VECTOR_ABORT_PREFETCH = 0x0000000C,  // Abort mode
   VECTOR_ABORT_DATA     = 0x00000010,  // Abort mode
   VECTOR_RESERVED       = 0x00000014,  // ?? This is only used for a 26 bit program (backwards compatibility)
   VECTOR_IRQ            = 0x00000018,  // IRQ mode
   VECTOR_FIQ            = 0x0000001C   // FIQ mode
};

class ARM60Vectors
{
public:
   ARM60Vectors ();
   ~ARM60Vectors ();

   uint* Vector (VectorType vect);

private:
   uint* m_vects;
};

#endif // _INC_ARM60VECTORS

