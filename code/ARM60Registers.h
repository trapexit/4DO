#ifndef _INC_ARM60REGISTERH
#define _INC_ARM60REGISTERH
//////////////////////////////////////////////////

#include "types.h"
#include "ARM60PSRegister.h"

#define REG_COUNT 37

enum RegisterType
{
   ARM60_R00 = 0,
   ARM60_R01 = 1,
   ARM60_R02 = 2,
   ARM60_R03 = 3,
   ARM60_R04 = 4,
   ARM60_R05 = 5,
   ARM60_R06 = 6,
   ARM60_R07 = 7,
   ARM60_R08 = 8,
   ARM60_R09 = 9,
   ARM60_R10 = 10,
   ARM60_R11 = 11,
   ARM60_R12 = 12,
   ARM60_R13 = 13,
   ARM60_R14 = 14,
   ARM60_PC = 15,
   ARM60_CPSR = 16,
   ARM60_SPSR = 17
};

enum InternalRegisterType
{
   IR_R00 = 0,
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

class ARM60Registers
{
public:
   ARM60Registers ();
   ~ARM60Registers ();

   uint* Reg (RegisterType reg);
   uint* Reg (InternalRegisterType reg);

   ARM60Register*   PC ();
   ARM60PSRegister* CPSR ();

private:
   InternalRegisterType GetInternalRegisterNum (RegisterType reg);

   uint m_regs[ REG_COUNT ];

   ARM60Register*   m_PC;
   ARM60PSRegister* m_CPSR;
};

#endif // _INC_ARM60REGISTERH