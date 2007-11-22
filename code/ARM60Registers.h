#include "types.h"

#define REG_COUNT 37

class ARM60Registers
{
public:
   ARM60Registers ();
   ~ARM60Registers ();

private:
   uint* m_regs;
};

enum RegisterType
{
   R00 = 0,
   R01 = 1,
   R02 = 2,
   R03 = 3,
   R04 = 4,
   R05 = 5,
   R06 = 6,
   R07 = 7,
   R08 = 8,
   R09 = 9,
   R10 = 10,
   R11 = 11,
   R12 = 12,
   R13 = 13,
   R14 = 14,
   PC = 15,
   CPSR = 16,
   SPSR = 17
};

enum InternalRegisterType
{
   IR_R00,
   IR_R01,
   IR_R02,
   IR_R03,
   IR_R04,
   IR_R05,
   IR_R06,
   IR_R07,
   IR_R08,
   IR_R09,
   IR_R10,
   IR_R11,
   IR_R12,
   IR_R13,
   IR_R14, // Potential copy of PC for Branch and Links (and interrupts)
   IR_PC, // Program counter (PC)
   IR_R08_FIQ,
   IR_R09_FIQ,
   IR_R10_FIQ,
   IR_R11_FIQ,
   IR_R12_FIQ,
   IR_R13_FIQ,
   IR_R14_FIQ,
   IR_R13_SVC,
   IR_R14_SVC,
   IR_R13_ABT,
   IR_R14_ABT,
   IR_R13_IRQ,
   IR_R14_IRQ,
   IR_R13_UND,
   IR_R14_UND,
   IR_CPSR,
   IR_SPSR_FIQ, // SPSR's are loaded with the value of CPSR when an exception occurs.
   IR_SPSR_SVC,
   IR_SPSR_ABT,
   IR_SPSR_IRQ,
   IR_SPSR_UND
};
