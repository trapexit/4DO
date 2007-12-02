#include "ARM60Registers.h"

ARM60Registers::ARM60Registers ()
{
   m_regs = new uint [REG_COUNT];

   // Potentially pointless initialization.
   for (int x = 0; x < REG_COUNT; x++)
   {
      m_regs [x] = 0;
   }

   m_PC = new ARM60Register (&m_regs [(int) ARM60_PC]);
   m_CPSR = new ARM60PSRegister (&m_regs [(int) ARM60_CPSR]);
}

ARM60Registers::~ARM60Registers ()
{
   delete m_regs;
   delete m_PC;
   delete m_CPSR;
}

ARM60Register* ARM60Registers::PC ()
{
   return m_PC;
}

ARM60PSRegister* ARM60Registers::CPSR ()
{
   return m_CPSR;
}

uint* ARM60Registers::Reg (RegisterType reg)
{
   int regNum = (int) reg;
   int retVal = -1;

   if ((regNum >= ARM60_R00 && regNum <= ARM60_R07) || regNum == ARM60_PC)
   {
      retVal = regNum;
   }

   if (reg == ARM60_CPSR)
   {
      retVal = (int) IR_CPSR;
   }

   if (reg == ARM60_SPSR)
   {
      switch (m_CPSR->GetCPUMode ())
      {
      case CPUMODE_USR:
         // NOTE: This is invalid!!
         
      case CPUMODE_FIQ:
         retVal = (int) IR_SPSR_FIQ;
         break;

      case CPUMODE_SVC:
         retVal = (int) IR_SPSR_SVC;
         break;

      case CPUMODE_ABT:
         retVal = (int) IR_SPSR_ABT;
         break;

      case CPUMODE_IRQ:
         retVal = (int) IR_SPSR_IRQ;
         break;

      case CPUMODE_UND:
         retVal = (int) IR_SPSR_UND;
         break;
      }
   }

   // More shared registers (FIQ32 has special cases)
   if (regNum >= ARM60_R08 && regNum <= ARM60_R12)
   {
      if (m_CPSR->GetCPUMode () == CPUMODE_FIQ)
      {
         switch (regNum)
         {
         case ARM60_R08:
            retVal = (int) IR_R08_FIQ;
            break;
            
         case ARM60_R09:
            retVal = (int) IR_R09_FIQ;
            break;
            
         case ARM60_R10:
            retVal = (int) IR_R10_FIQ;
            break;
            
         case ARM60_R11:
            retVal = (int) IR_R11_FIQ;
            break;
            
         case ARM60_R12:
            retVal = (int) IR_R12_FIQ;
            break;
         }
      }
      else
      {
         // All other modes share these registers.
         retVal = regNum;
      }
   }

   // Mode-specific registers.
   switch (m_CPSR->GetCPUMode ())
   {
   case CPUMODE_USR:
      switch (regNum)
      {
      case ARM60_R13:
         retVal = (int) IR_R13;
         break;
      
      case ARM60_R14:
         retVal = (int) IR_R14;
         break;
      }
      break;
   
   case CPUMODE_FIQ:
      switch (regNum)
      {
      case ARM60_R13:
         retVal = (int) IR_R13_FIQ;
         break;
      
      case ARM60_R14:
         retVal = (int) IR_R14_FIQ;
         break;
      }
      break;

   case CPUMODE_SVC:
      switch (regNum)
      {
      case ARM60_R13:
         retVal = (int) IR_R13_SVC;
         break;
      
      case ARM60_R14:
         retVal = (int) IR_R14_SVC;
         break;
      }
      break;

   case CPUMODE_ABT:
      switch (regNum)
      {
      case ARM60_R13:
         retVal = (int) IR_R13_ABT;
         break;
      
      case ARM60_R14:
         retVal = (int) IR_R14_ABT;
         break;
      }
      break;

   case CPUMODE_IRQ:
      switch (regNum)
      {
      case ARM60_R13:
         retVal = (int) IR_R13_IRQ;
         break;
      
      case ARM60_R14:
         retVal = (int) IR_R14_IRQ;
         break;
      }
      break;

   case CPUMODE_UND:
      switch (regNum)
      {
      case ARM60_R13:
         retVal = (int) IR_R13_UND;
         break;
      
      case ARM60_R14:
         retVal = (int) IR_R14_UND;
         break;
      }
      break;
   }

   if (retVal == -1)
   {
      // Um. well. shit.
      return &m_regs [0];
   }
   else
   {
      return &m_regs [retVal];
   }
}