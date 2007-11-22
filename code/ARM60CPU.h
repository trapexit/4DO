#include "types.h"

enum CPUMode
{
   USR, // Normal program execution state.
   FIQ, // Designed to support a data transfer or channel process
   IRQ, // Used for general purpose interrupt handling
   SVC, // A protected mode for the operating system.
   ABT, // Entered after a data or instruction prefetch abort.
   UND, // Entered when an undefined instruction is executed.
   Invalid // NOTE: I'm unsure if this will ever happen, but I don't want it to crash on invalid data.
};