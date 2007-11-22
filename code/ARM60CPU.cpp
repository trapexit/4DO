#include <iostream.h>

#include "types.h"
#include "ARM60CPU.h"
#include "ARM60REGISTER.h"
#include "ARM60REGISTERS.h"

ARM60CPU::ARM60CPU ()
{
   m_reg = new ARM60Registers ();
}

ARM60CPU::~ARM60CPU ()
{
   delete m_reg;
}

void ProcessInstruction (uint instruction)
{
   if      ((instruction & 0x0C000000) == 0x00000000)
   {
      // Data Processing PSR Transfer (00)
   }
   else if ((instruction & 0x0C000000) == 0x04000000)
   {
      // Single Data Transfer (01)
   }
   else if ((instruction & 0x0E000000) == 0x06000000)
   {
      // Undefined (011)
   }
   else if ((instruction & 0x0E000000) == 0x08000000)
   {
      // Block Data Transfer (100)
   }
   else if ((instruction & 0x0E000000) == 0x0A000000)
   {
      // Branch (101)

   }
   else if ((instruction & 0x0E000000) == 0x0C000000)
   {
      // Coproc Data Transfer (110)
   }
   else if ((instruction & 0x0F000000) == 0x0F000000)
   {
      // Software Interrupt
   }
   else if ((instruction & 0x0F000010) == 0x0E000010)
   {
      // Coproc Register Transfer (1110 ... 1)
   }
   else if ((instruction & 0x0F000010) == 0x0E000000)
   {
      // Coproc Data Operation (1110 ... 0)
   }
   else if ((instruction & 0x0F800000) == 0x01000000)
   {
      // Single Data Swap
   }
   else if ((instruction & 0x0FC00000) == 0x00000000)
   {
      // Multiply
   }
   else
   {
      // Aw shit. Now what do we do?
   }
}

void ARM60CPU::ProcessBranch (uint instruction)
{
   // Check condition Field.
   if (! CheckCondition (inst))
      return;

   ///////////////////////
   bool link;
   uint offset;

   link = (instruction & 0x01000000) == 0x01000000;
   offset = (instruction & 0x00FFFFFF);
}

bool ARM60CPU::CheckCondition (uint instruction)
{
   ARM60PSRegister* CPSR = m_reg->CPSR ();
   uint cond;

   cond = instruction & 0xF0000000;

   switch (cond)
   {
   case 0x00000000:
      // EQ - Z set (equal)
      return CPSR->GetZero ();
   
   case 0x10000000:
      // NE - Z clear (not equal)
      return !CPSR->GetZero ();
   
   case 0x20000000:
      // CS - C set (unsigned higher or same)
      return CPSR->GetCarry ();
   
   case 0x30000000:
      // CC - C clear (unsigned lower)
      return !CPSR->GetCarry ();
   
   case 0x40000000:
      // MI - N set (negative)
      return CPSR->GetNegative ();
   
   case 0x50000000:
      // PL - N clear (positive or zero)
      return !CPSR->GetNegative ();
   
   case 0x60000000:
      // VS - V set (overflow)
      return CPSR->GetOverflow ();
   
   case 0x70000000:
      // VC - V clear (no overflow)
      return !CPSR->GetOverflow ();
   
   case 0x80000000:
      // HI - C set and Z clear (unsigned higher)
      return CPSR->GetCarry () && (!CPSR->GetZero ());
   
   case 0x90000000:
      // LS - C clear or Z set (unsigned lower or same)
      return (!CPSR->GetCarry ()) || CPSR->GetZero ();
   
   case 0xA0000000:
      // GE - N set and V set, or N clear and V clear (greater or equal)
      return (CPSR->GetNegative ()&& CPSR->GetOverflow ())
            || ((!CPSR->GetNegative ()) && (!CPSR->GetOverflow ()));
   
   case 0xB0000000:
      // LT - N set and V clear, or N clear and V set (less than)
      return (CPSR->GetNegative  () && (!CPSR->GetOverflow ()))
            || ((!CPSR->GetNegative ()) && CPSR->GetOverflow ());
   
   case 0xC0000000:
      // GT - Z clear, and either N set and V set, or N clear and V clear (greater than)
      return (!CPSR->GetZero ())
            && ((CPSR->GetNegative  () && CPSR->GetOverflow ())
               || ((!CPSR->GetNegative ()) && (!CPSR->GetOverflow ())));
   
   case 0xD0000000:
      // LE - Z set, or N set and V clear, or N clear and V set (less than or equal)
      return CPSR->GetZero ()
            || (CPSR->GetNegative  () && (!CPSR->GetOverflow ()))
            || ((!CPSR->GetNegative ()) && CPSR->GetOverflow ());

   case 0xE0000000:
      // AL - always
      return true;

   case 0xF0000000:
      // NV - never
      
      // NOTE: The documentation specifies that the NV condition should
      //       not be used because it will be redifined in later ARM
      //       versions.
      return false;
   }

   // NOTE: The documentation specifies that the absence of a condition
   //       code acts as though "always" had been specified, although
   //       I don't see how this is possible.
   return true;
}