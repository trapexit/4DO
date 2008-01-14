#include "ARM60CPU.h"

ARM60CPU::ARM60CPU ()
{
   REG = new ARM60Registers ();
   m_vect = new ARM60Vectors ();
   m_pipe = new ARM60Pipeline (3);
}

ARM60CPU::~ARM60CPU ()
{
   delete REG;
   delete m_vect;
   delete m_pipe;
}

void ARM60CPU::DoSingleInstruction ()
{
   this->DoSingleInstruction (DMA->GetValue (*(REG->PC()->Value)));
}

void ARM60CPU::DoSingleInstruction (uint instruction)
{
   this->ProcessInstruction (instruction);
}

void ARM60CPU::ProcessInstruction (uint instruction)
{
   /////////////////////
   // Some of these (especially those starting with 00
   // are rather cryptic, but I am sure they're a
   // deterministic way to figure the type of instruction.
   // So, don't toy with them thinking there's a faster way.
   //
   // Hopefully I did it right, or I'll have to do it
   // all over again...

   if      ((instruction & 0x0FC000F0) == 0x00000090)
   {
      // Multiply
      this->ProcessMultiply (instruction);
   }
   else if ((instruction & 0x0C000000) == 0x00000000)
   {
      // Starts with 00...
      if ((instruction & 0x01900000) == 0x01000000)
      {
         // Either MRS/MSR(PRS) or SWP
         if ((instruction & 0x02200080) == 0x00000080)
         {
            // SWP (Single Data Swap)
            this->ProcessSingleDataSwap (instruction);
         }
         else
         {
            // MRS/MSR (PRS)
            this->ProcessPSRTransfer (instruction);
         }
      }
      else
      {
         // Data Processing
         this->ProcessDataProcessing (instruction);
      }
   }
   else if ((instruction & 0x0C000000) == 0x04000000)
   {
      // Single Data Transfer (01)
      this->ProcessSingleDataTransfer (instruction);
   }
   else if ((instruction & 0x0E000000) == 0x06000000)
   {
      // Undefined (011)
      this->ProcessUndefined (instruction);
   }
   else if ((instruction & 0x0E000000) == 0x08000000)
   {
      // Block Data Transfer (100)
      this->ProcessBlockDataTransfer (instruction);
   }
   else if ((instruction & 0x0E000000) == 0x0A000000)
   {
      // Branch (101)
      this->ProcessBranch (instruction);
   }
   else if ((instruction & 0x0E000000) == 0x0C000000)
   {
      // Coproc Data Transfer (110)
      this->ProcessCoprocessorDataTransfers (instruction);
   }
   else if ((instruction & 0x0F000000) == 0x0F000000)
   {
      // Software Interrupt (1111)
      this->ProcessSoftwareInterrupt (instruction);
   }
   else if ((instruction & 0x0F000010) == 0x0E000010)
   {
      // Coproc Register Transfer (1110 ... 1)
      this->ProcessCoprocessorRegisterTransfers (instruction);
   }
   else if ((instruction & 0x0F000010) == 0x0E000000)
   {
      // Coproc Data Operation (1110 ... 0)
      this->ProcessCoprocessorDataOperations (instruction);
   }
   else
   {
      // Aw shit. Now what do we do?
   }
}

////////////////////////////////////////////////////////////
// Branch (B, BL)
////////////////////////////////////////////////////////////
void ARM60CPU::ProcessBranch (uint instruction)
{
   #ifdef __WXDEBUG__
   wxLogMessage ("Processed Branch");
   LastResult = "Branch";
   #endif
   
   // Check condition Field.
   if (! CheckCondition (instruction))
      return;

   //////////////////////
   //  3         2         1         0
   // 10987654321098765432109876543210
   // Cond   L
   //     101 [________Offset________]
   //////////////////////
   int  offset;

   if ((instruction & 0x01000000) == 0x01000000)
   {
      // TODO: Check on prefetch logic.
      *(REG->Reg (ARM60_R14)) = *(REG->PC ()->Value);
   }
   
   // Offset is bit shifted left two, then sign extended to 32 bits.
   offset = (instruction & 0x00FFFFFF) << 2;
   if ((offset & 0x02000000) > 0)
   {
      offset &= 0xFC000000;
   }
   (*(REG->PC ()->Value)) += offset;
}

////////////////////////////////////////////////////////////
// Data Processing
////////////////////////////////////////////////////////////
void ARM60CPU::ProcessDataProcessing (uint instruction)
{
   #ifdef __WXDEBUG__
   wxLogMessage ("Processed Branch");
   LastResult = "Data Proc";
   #endif
   
   // Check condition Field.
   if (! CheckCondition (instruction))
      return;

   //////////////////////
   //  3         2         1         0
   // 10987654321098765432109876543210
   // Cond  I    S    [Rd]
   //     00 OpCd [Rn]    [ Operand 2]
   //////////////////////
   
   //////////////////////
   bool newCarry = false;
   bool setCond;
   bool writeResult = true; // NOTE: The assembler allegegly always sets S to false here.
   bool isLogicOp = true;
   uint op1;
   uint op2;
   uint result = 0;
   int  opCode;
   int  regDest;
   int  shift;

   opCode = (instruction & 0x01E00000) >> 21;
   setCond = (instruction & 0x00100000) > 0;
   
   regDest = (instruction & 0x000F000) >> 12;

   ////////////////////////////
   // Get first operand value.
   op1 = *(REG->Reg ((RegisterType) (instruction & 0x000F0000)));
   
   ////////////////////////////
   // Get second operand value.

   // TODO: When the operand is R15 (PC), prefetch may need to be added.

   // Check Immediate Operand.
   if ((instruction & 0x02000000) > 0)
   {
      // Immediate - operand 2 is an immediate value.
      //  1 0 9 8 7 6 5 4 3 2 1 0
      // [Rotate][ImmediateValue]
      shift = ((instruction & 0x00000F00) >> 8) * 2; // NOTE: it rotates by double this amt.
      op2 = (instruction & 0x000000FF);

      // Rotate value.
      op2 = (op2 >> shift) | (op2 << (32 - shift));
   }
   else
   {
      // Not immediate - Operand 2 is a shifted register.
      op2 = ReadShiftedRegisterOperand (instruction, &newCarry);
   }
   
   ///////////////////////
   // Handle each type of operation.

   switch (opCode)
   {
   case 0x0:
      // AND
      result = op1 & op2;
      break;
   
   case 0x1:
      // EOR
      result = op1 ^ op2;
      break;
   
   case 0x2:
      // SUB
      isLogicOp = false;
      result = DoAdd (op1, (~op2)+1, false, &newCarry);
      break;
   
   case 0x3:
      // RSB
      isLogicOp = false;
      result = DoAdd (op2, (~op1)+1, false, &newCarry);
      break;
   
   case 0x4:
      // ADD
      isLogicOp = false;
      result = DoAdd (op1, op2, false, &newCarry);
      break;
   
   case 0x5:
      // ADC
      isLogicOp = false;
      result = DoAdd (op1, op2, REG->CPSR ()->GetCarry (), &newCarry);
      break;
   
   case 0x6:
      // SBC
      isLogicOp = false;
      result = DoAdd (op1, (~op2)+1, REG->CPSR ()->GetCarry (), &newCarry);
      break;
   
   case 0x7:
      // RSC
      isLogicOp = false;
      result = DoAdd (op2, (~op1)+1, REG->CPSR ()->GetCarry (), &newCarry);
      break;
   
   case 0x8:
      // TST
      writeResult = false;
      result = op2 & op1;
      break;
   
   case 0x9:
      // TEQ
      writeResult = false;
      result = op2 ^ op1;
      break;
   
   case 0xA:
      // CMP
      writeResult = false;
      isLogicOp = false;
      result = DoAdd (op1, -(int)op2, REG->CPSR ()->GetCarry (), &newCarry);
      break;
   
   case 0xB:
      // CMN
      writeResult = false;
      isLogicOp = false;
      result = DoAdd (op1, -(int)op2, REG->CPSR ()->GetCarry (), &newCarry);
      break;
   
   case 0xC:
      // ORR
      result = op2 | op1;
      break;
   
   case 0xD:
      // MOV
      result = op2;
      break;

   case 0xE:
      // BIC
      result = op1 & (~op2);
      break;

   case 0xF:
      // MVN
      result = ~op2;
      break;

   }

   ///////////////////////
   // Write result if necessary.
   if (writeResult)
   {
      (*(REG->Reg ((RegisterType) regDest))) = result;
   }

   ///////////////////////
   // Write condition flags if instructed to.

   if (setCond)
   {
      if (regDest == (int) ARM60_PC)
      {
         if (REG->CPSR ()->GetCPUMode() == CPUMODE_USR)
         {
            // Hm, this shouldn't happen.
            // TODO: Error?
         }
         else
         {
            // SPSR of current mode is written to CPSR;
            *(REG->Reg (ARM60_CPSR)) = *(REG->Reg (ARM60_SPSR));
         }
      }
      
      if (isLogicOp)
      {
         // Overflow bit is not changed in this scenario.
         REG->CPSR ()->SetCarry (newCarry);
         REG->CPSR ()->SetZero (result == 0);
         REG->CPSR ()->SetNegative ((result & 0x80000000) > 0);
      }
      else
      {
         // TODO: Double-check carry and overflow logic!!
         
         // Overflow bit is specific to two's compliment operations.
         // It will be set if the sign of the two operands differs
         // from the sign of the result.
         if ((op1 & 0x80000000) == (op2 & 0x80000000))
         {
            REG->CPSR ()->SetOverflow (!
                  ((result & 0x80000000) == (op1 & 0x80000000)));
         }
         else
         {
            // Overflow is impossible.
            REG->CPSR ()->SetOverflow (false);
         }
         REG->CPSR ()->SetCarry (newCarry);
         REG->CPSR ()->SetZero (result == 0);
         REG->CPSR ()->SetNegative ((result & 0x80000000) > 0);
      }
   }
}

////////////////////////////////////////////////////////////
// PSR Transfer (MRS, MSR)
////////////////////////////////////////////////////////////
void ARM60CPU::ProcessPSRTransfer (uint instruction)
{
   #ifdef __WXDEBUG__
   LastResult = "PSR Trans";
   #endif

   // Check condition Field.
   if (! CheckCondition (instruction))
      return;

   //////////////////////////
   uint sourceVal;
   int  rotate;

   /////////////////
   // Determine operation
   switch (instruction & 0x003F0000)
   {
   case 0x000F0000:
      // MRS - transfer PSR contents to a register
      if ((instruction & 0x00400000) > 0)
      {
         // Source is SPSR
         *(REG->Reg ((RegisterType) ((instruction & 0x0000F000) > 12))) = 
            *(REG->Reg (ARM60_SPSR));
      }
      else
      {
         // Source is CPSR
         *(REG->Reg ((RegisterType) ((instruction & 0x0000F000) > 12))) = 
            *(REG->Reg (ARM60_CPSR));
      }
      break;

   case 0x00290000:
      // MSR - transfer register contents to PSR
      if ((instruction & 0x00400000) > 0)
      {
         // Destination is SPSR
         *(REG->Reg (ARM60_SPSR)) = 
            *(REG->Reg ((RegisterType) (instruction & 0x0000000F)));
      }
      else
      {
         // Destination is CPSR
         *(REG->Reg (ARM60_CPSR)) = 
            *(REG->Reg ((RegisterType) (instruction & 0x0000000F)));
      }
      break;

   case 0x00280000:
      // MSR - transfer register contents or immediate value to PSR (flag bits only)
      
      //////////////
      // Get source.
      if ((instruction & 0x02000000) > 0)
      {
         // Rotated immediate value.
         rotate = ((instruction & 0x00000F00) >> 8) * 2;
         sourceVal = (instruction & 0x000000FF);

         // Rotate value.
         sourceVal = (sourceVal >> rotate) | (sourceVal << (32 - rotate));

      }
      else
      {
         // Register value.
         sourceVal = *(REG->Reg ((RegisterType) (instruction & 0x0000000F)));
      }
      
      //////////////
      // Give to destination.
      if ((instruction & 0x00400000) > 0)
      {
         // Destination is SPSR
         *(REG->Reg (ARM60_SPSR)) = sourceVal;
      }
      else
      {
         // Destination is CPSR
         *(REG->Reg (ARM60_CPSR)) = sourceVal;
      }
      break;
   }
}

////////////////////////////////////////////////////////////
// Multiply (MUL, MLA)
////////////////////////////////////////////////////////////
void ARM60CPU::ProcessMultiply (uint instruction)
{
   #ifdef __WXDEBUG__
   LastResult = "Multiply";
   #endif
 
   // Check condition Field.
   if (! CheckCondition (instruction))
      return;

   //////////////////////
   //  3         2         1         0
   // 10987654321098765432109876543210
   // Cond      A [Rd]    [Rs]    [Rm]
   //     000000 S    [Rn]    1001 
   
   ///////////////////////////

   uint result;
   RegisterType Rd;
   RegisterType Rn;
   RegisterType Rs;
   RegisterType Rm;
   bool accumulate;
   
   ////////////////////////////
   Rd = (RegisterType) (instruction & 0x000F0000);
   Rn = (RegisterType) (instruction & 0x0000F000);
   Rs = (RegisterType) (instruction & 0x00000F00);
   Rm = (RegisterType) (instruction & 0x0000000F);
   accumulate = (instruction & 0x00200000) > 0;

   result = (*(REG->Reg (Rm))) * (*(REG->Reg (Rs)));

   if (Rm == Rd)
   {
      // Uh oh! Lowest-order register is also the destination!
      //
      // This is documented as a bad idea, since the source register is 
      // read multiple times throughout the multiplication.
      //
      // MUL returns 0
      // MLA returns a "meaningless result"

      result = 0;
   }
   
   // Optionally add Rn
   if (accumulate)
   {
      result += *(REG->Reg (Rn));
   }

   // Set the result to Rd
   *(REG->Reg ((RegisterType) (instruction & 0x000F0000))) = result;
   
   // Optionally set CPSR values.
   if ((instruction & 0x00100000) > 0)
   {
      // Set CPSR values.
      
      // NOTE: Overflow is unaffected.

      // NOTE: Carry is set to a "meaningless value".
      REG->CPSR ()->SetCarry (0);
      REG->CPSR ()->SetZero (result == 0);
      REG->CPSR ()->SetNegative ((result & 0x80000000) > 0);
   }
}

////////////////////////////////////////////////////////////
// Single Data Transfer (LDR, STR)
////////////////////////////////////////////////////////////
void ARM60CPU::ProcessSingleDataTransfer (uint instruction)
{
   #ifdef __WXDEBUG__
   LastResult = "Single DT";
   #endif

   // Check condition Field.
   if (! CheckCondition (instruction))
      return;

   //////////////////////
   //  3         2         1         0
   // 10987654321098765432109876543210
   // Cond  I U W [Rn]    [  Offset  ]
   //     01 P B L    [Rd]
   //            (base)
   //                (dest)
   ///////////////////////////

   bool  newCarry = false;
   int   offset;
   uint* baseReg;
   uint  address;

   ////////////////////////////////////////////////////////

   baseReg = REG->Reg ((RegisterType) (0x000F0000 >> 16));

   // TODO: Special cases around use of R15 (prefetch).
   // TODO: Abort logic.

   ////////////////////
   // Read offset.
   if ((instruction & 0x02000000) > 0)
   {
      // NOTE: The register-specified shift amounts are not available
      //       in this instruction class.
      offset = ReadShiftedRegisterOperand (instruction, &newCarry);
   }
   else
   {
      // Offset is an immediate value.
      offset = instruction & 0x00000FFF;
   }

   // Set offset positive/negative.
   if ((instruction & 0x00800000) == 0)
   {
      // We're supposed to subtract the offset.
      offset = -offset;
   }

   if ((instruction & 0x01000000) > 0)
   {
      // Pre-indexed.
      // offset modification is performed before the base is used as the address.
      
      if ((instruction & 0x00200000) > 0)
      {
         // Write it back to the base register before we begin.
         *(baseReg) += offset;
         address = *(baseReg);
      }
      else
      {
         // Just use the base and offset. The base register is preserved.
         address = *(baseReg) + offset;
      }
   }
   else
   {
      // Post-indexed. 
      // offset modification is performed after the base is used as the address.
      // I don't see what this means... doesn't mean the offset just isn't done?
      address = *(baseReg);

      // NOTE: Use of the write-back bit is meaningless here except for 
      //       some mention of this as a practice in privileged mode, where
      //       setting the W bit forces non-privileged mode for the transfer,
      //       "allowing the operating system to generate a user address
      //       in a system where the memory management hardware makes suitable
      //       use of this hardware"
   }

   ////////////////////
   if ((instruction & 0x00100000) > 0)
   {
      *(REG->Reg((RegisterType) (0x0000F000 >> 12))) = 
         DoLDR (address, (instruction & 0x00400000) > 0);
   }
   else
   {
      DoSTR (address, (RegisterType) (0x0000F000 >> 12), (instruction & 0x00400000) > 0);
   }
}

////////////////////////////////////////////////////////////
// Block Data Transfer (LDM, STM)
////////////////////////////////////////////////////////////
void ARM60CPU::ProcessBlockDataTransfer (uint instruction)
{
   #ifdef __WXDEBUG__
   LastResult = "Block DT";
   #endif

   // Check condition Field.
   if (! CheckCondition (instruction))
      return;

   //////////////////////
   //  3         2         1         0
   // 10987654321098765432109876543210
   // Cond   P S L    [ Register List]
   //     100 U W [Rn]
   //
   // P = Pre/Post indexing   = 0x01000000
   // U = Up/Down (1=up)      = 0x00800000
   // S = PSR & force user    = 0x00400000
   // W = Write-back (1=yes)  = 0x00200000
   // L = Load/Store (1=load) = 0x00100000
   ///////////////////////////

   bool  preIndex;
   bool  isWriteBack;
   bool  isPSR;
   bool  isLoad;
   bool  isR15used;
   int   increment;
   int   registerCount=0;
   int   reg;
   uint* baseReg;
   uint* destReg;
   uint  address;

   // Going up or down?
   increment = (instruction & 0x00800000) > 0 ? 4 : -4;
   
   // Find out how many registers we're storing.
   for (reg = 0; reg < 16; reg++)
   {
      if ((instruction & (int) pow((double) 2, reg)) > 0)
      {
         registerCount++;
      }
   }

   // See if R15 is being used.
   isR15used = (instruction & 0x00008000) > 0;  
   
   // Get some more parameters.
   preIndex = (instruction & 0x01000000) > 0; 
   isWriteBack = (instruction & 0x00200000) > 0;
   isPSR = (instruction & 0x00400000) > 0;
   isLoad = (instruction & 0x00100000) > 0;
   
   // Get base address.
   baseReg = REG->Reg ((RegisterType) (instruction & 0x000F0000));
   address = *baseReg;

   /////////////////////////
   // Start storing/loading registers.
   for (reg = 0; reg < 16; reg++)
   {
      if ((instruction & (int) pow((double) 2, reg)) > 0)
      {
         ////////////////////
         // Perform preindexing?
         if (preIndex)
         {
            // We are preindexing. Update address first.
            address += increment;
            if (isWriteBack)
               *baseReg = address;
         }
         
         ////////////////////
         // Determine destination/source.
         // The selected register is affected by the S bit and use of R15.
         if (isPSR)
         {
            if (isR15used)
            {
               if (isLoad)
               {
                  // SPSR_mode is transferred to CPSR at the same as R15 is loaded.

                  // In all other respectes, this mode is normal.
                  destReg = REG->Reg ((RegisterType) reg);
               }
               else
               {
                  // Use the user mode's registers regardless of current mode.
                  destReg = REG->Reg ((InternalRegisterType) reg);
               }
            }
            else
            {
               // Use the user mode's registers regardless of current mode.
               destReg = REG->Reg ((InternalRegisterType) reg);
            }
         }
         else
         {
            // Normal operation.
            destReg = REG->Reg ((RegisterType) reg);
         }

         ////////////////////
         // Perform operation here.
         if (isLoad)
         {
            *(destReg) = DMA->GetValue (address);
            if (isPSR && isR15used && isLoad)
            {
               // SPSR_mode is transferred to CPSR at the same as R15 is loaded.
               *(REG->Reg (ARM60_CPSR)) = *(REG->Reg (ARM60_SPSR));
            }
         }
         else
         {
            DMA->SetValue (address, *(destReg));
         }

         ////////////////////
         // Perform postindexing?
         if (!preIndex)
         {
            // We are postindexing. Update address last.
            address += increment;
            if (isWriteBack)
               *baseReg = address;
         }
      }
   }
}

////////////////////////////////////////////////////////////
// Single data swap (SWP)
////////////////////////////////////////////////////////////
void ARM60CPU::ProcessSingleDataSwap (uint instruction)
{
   #ifdef __WXDEBUG__
   LastResult = "Data Swap";
   #endif
   
   // Check condition Field.
   if (! CheckCondition (instruction))
      return;

   ///////////////////////////
   //  3         2         1         0
   // 10987654321098765432109876543210
   // Cond     B  [Rn]    00001001
   //     00010 00    [Rd]        [Rm]
   //       (Byte?)  (Dest)     (Source)
   //            (Base)
   //
   // B = Byte/Word (1=byte)  = 0x00400000
   ///////////////////////////

   RegisterType baseReg;
   RegisterType destReg;
   RegisterType sourceReg;
   uint         value;
   uint         address;
   bool         isByte;

   baseReg = (RegisterType) (instruction & 0x000F0000 >> 16);
   sourceReg = (RegisterType) (instruction & 0x0000000F);
   destReg = (RegisterType) (instruction & 0x0000F000 >> 12);
   isByte = (instruction & 0x00400000) > 0;

   ///////
   // This is supposed to be an atomic operation. 
   // The DMA should check this LOCK bit!

   LOCK = true;
   
   address = *(REG->Reg (baseReg));
   
   value = DoLDR (address, isByte);
   DoSTR (address, sourceReg, isByte);
   *(REG->Reg (destReg)) = value;

   LOCK = false;
}

////////////////////////////////////////////////////////////
// Software Interrupt (SWI)
////////////////////////////////////////////////////////////
void ARM60CPU::ProcessSoftwareInterrupt (uint instruction)
{
   #ifdef __WXDEBUG__
   LastResult = "Software Int";
   #endif
   
   // Check condition Field.
   if (! CheckCondition (instruction))
      return;

   ///////////////////////////
   //  3         2         1         0
   // 10987654321098765432109876543210
   // Cond    [Comment Field(ignored)]
   //     1111
   ///////////////////////////
   
   //REG->

   //TODO: Software interrupt.
}

////////////////////////////////////////////////////////////
// Coproccesor data operations (CDP)
////////////////////////////////////////////////////////////
void ARM60CPU::ProcessCoprocessorDataOperations (uint instruction)
{
   #ifdef __WXDEBUG__
   LastResult = "Coproc DO";
   #endif
   
   // Check condition Field.
   if (! CheckCondition (instruction))
      return;

   ///////////////////////////
   //  3         2         1         0
   // 10987654321098765432109876543210
   // Cond
   //     
   ///////////////////////////
}

////////////////////////////////////////////////////////////
// Coproccesor data transfers (LDC, STC)
////////////////////////////////////////////////////////////
void ARM60CPU::ProcessCoprocessorDataTransfers (uint instruction)
{
   #ifdef __WXDEBUG__
   LastResult = "Coproc DT";
   #endif

   // Check condition Field.
   if (! CheckCondition (instruction))
      return;

   ///////////////////////////
   //  3         2         1         0
   // 10987654321098765432109876543210
   // Cond
   //     
   ///////////////////////////
}

////////////////////////////////////////////////////////////
// Coproccesor register transfers (MRC, MCR)
////////////////////////////////////////////////////////////
void ARM60CPU::ProcessCoprocessorRegisterTransfers (uint instruction)
{
   #ifdef __WXDEBUG__
   LastResult = "Coproc Reg";
   #endif

   // Check condition Field.
   if (! CheckCondition (instruction))
      return;

   ///////////////////////////
   //  3         2         1         0
   // 10987654321098765432109876543210
   // Cond    CPO CRn_    CP#_   1
   //     1110   L    [Rd]    CP_ CRm_
   ///////////////////////////
}

////////////////////////////////////////////////////////////
// Undefined instruction.
////////////////////////////////////////////////////////////
void ARM60CPU::ProcessUndefined (uint instruction)
{
   #ifdef __WXDEBUG__
   LastResult = "Undefined";
   #endif

   // Check condition Field.
   if (! CheckCondition (instruction))
      return;

   ///////////////////////////
   //  3         2         1         0
   // 10987654321098765432109876543210
   // Cond   xxxxxxxxxxxxxxxxxxxx xxxx
   //     011                    1
   ///////////////////////////
}

///////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////

uint ARM60CPU::DoLDR (uint address, bool isByte)
{
   uint value;

   value = DMA->GetValue (address);
   
   if (isByte)
   {
      // We are loading a byte.

      // Loaded value depends on word boundaries.
      switch (BIGEND ? (3 - (address % 4)) : (address % 4))
      {
      case 0:
         value = (value & 0x000000FF);
         break;
      case 1:
         value = (value & 0x0000FF00) >> 8;
         break;
      case 2:
         value = (value & 0x00FF0000) >> 16;
         break;
      case 3:
         value = (value & 0xFF000000) >> 24;
         break;
      }
      
      // and all other bits are set to zero... done with shifting.
   }
   else
   {
      // Loading a word.
      if (BIGEND)
      {
         switch (address % 4)
         {
         case 0:
            // All good
            break;
         case 1:
            // Rotate value.
            value = (value >> 8) | (value << 24);
            break;
         case 2:
            // Rotate value.
            value = (value >> 16) | (value << 16);
            break;
         case 3:
            // Rotate value.
            value = (value >> 24) | (value << 8);
            break;
         }
      }
      else
      {
         switch (address % 4)
         {
         case 0:
            // All good
            break;
         case 1:
            // Rotate addressed byte to bits 15 through 8?
            break;
         case 2:
            // Rotate value.
            value = (value >> 16) | (value << 16);
            break;
         case 3:
            // Rotate addressed byte to bits 15 through 8?
            value = (value >> 8) | (value << 24);
            break;
         }
      }
   }
   return value;
}

void ARM60CPU::DoSTR (uint address, RegisterType sourceReg, bool isByte)
{
   uint value;

   // Store to memory
   if (isByte)
   {
      // Storing a byte...
      // We store the bottom byte repeated four times.
      // TODO: Is this supposed be be affected by big endian vs little?
      value = *(REG->Reg (sourceReg)) & 0x000000FF;
      value = value | (value << 8) | (value << 16) | (value << 24);
      DMA->SetValue (address, value);
   }
   else
   {
      // Storing a word. Easy.
      DMA->SetValue (address, *(REG->Reg (sourceReg)));
   }
}

bool ARM60CPU::CheckCondition (uint instruction)
{
   ARM60PSRegister* CPSR = REG->CPSR ();
   uint cond;

   cond = instruction & 0xF0000000;

   switch (cond)
   {
   case 0x00000000:
      // EQ - Z set (equal)
      #ifdef __WXDEBUG__
      LastCond = "EQ";
      #endif
      return CPSR->GetZero ();
   
   case 0x10000000:
      // NE - Z clear (not equal)
      #ifdef __WXDEBUG__
      LastCond = "NE";
      #endif
      return !CPSR->GetZero ();
   
   case 0x20000000:
      // CS - C set (unsigned higher or same)
      #ifdef __WXDEBUG__
      LastCond = "CS";
      #endif
      return CPSR->GetCarry ();
   
   case 0x30000000:
      // CC - C clear (unsigned lower)
      return !CPSR->GetCarry ();
   
   case 0x40000000:
      // MI - N set (negative)
      #ifdef __WXDEBUG__
      LastCond = "MI";
      #endif
      return CPSR->GetNegative ();
   
   case 0x50000000:
      // PL - N clear (positive or zero)
      #ifdef __WXDEBUG__
      LastCond = "PL";
      #endif
      return !CPSR->GetNegative ();
   
   case 0x60000000:
      // VS - V set (overflow)
      #ifdef __WXDEBUG__
      LastCond = "VS";
      #endif
      return CPSR->GetOverflow ();
   
   case 0x70000000:
      // VC - V clear (no overflow)
      #ifdef __WXDEBUG__
      LastCond = "VC";
      #endif
      return !CPSR->GetOverflow ();
   
   case 0x80000000:
      // HI - C set and Z clear (unsigned higher)
      #ifdef __WXDEBUG__
      LastCond = "HI";
      #endif
      return CPSR->GetCarry () && (!CPSR->GetZero ());
   
   case 0x90000000:
      // LS - C clear or Z set (unsigned lower or same)
      #ifdef __WXDEBUG__
      LastCond = "LS";
      #endif
      return (!CPSR->GetCarry ()) || CPSR->GetZero ();
   
   case 0xA0000000:
      // GE - N set and V set, or N clear and V clear (greater or equal)
      #ifdef __WXDEBUG__
      LastCond = "GE";
      #endif
      return (CPSR->GetNegative ()&& CPSR->GetOverflow ())
            || ((!CPSR->GetNegative ()) && (!CPSR->GetOverflow ()));
   
   case 0xB0000000:
      // LT - N set and V clear, or N clear and V set (less than)
      #ifdef __WXDEBUG__
      LastCond = "LT";
      #endif
      return (CPSR->GetNegative  () && (!CPSR->GetOverflow ()))
            || ((!CPSR->GetNegative ()) && CPSR->GetOverflow ());
   
   case 0xC0000000:
      // GT - Z clear, and either N set and V set, or N clear and V clear (greater than)
      #ifdef __WXDEBUG__
      LastCond = "GT";
      #endif
      return (!CPSR->GetZero ())
            && ((CPSR->GetNegative  () && CPSR->GetOverflow ())
               || ((!CPSR->GetNegative ()) && (!CPSR->GetOverflow ())));
   
   case 0xD0000000:
      // LE - Z set, or N set and V clear, or N clear and V set (less than or equal)
      #ifdef __WXDEBUG__
      LastCond = "LE";
      #endif
      return CPSR->GetZero ()
            || (CPSR->GetNegative  () && (!CPSR->GetOverflow ()))
            || ((!CPSR->GetNegative ()) && CPSR->GetOverflow ());

   case 0xE0000000:
      // AL - always
      #ifdef __WXDEBUG__
      LastCond = "AL";
      #endif
      return true;

   case 0xF0000000:
      // NV - never
      #ifdef __WXDEBUG__
      LastCond = "NV";
      #endif
      
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

uint ARM60CPU::ReadShiftedRegisterOperand (uint instruction, bool* newCarry)
{
   RegisterType regShift;

   uint shiftType;
   uint op;
   int  shift;
   bool topBit;
   
   ////////////////////////
   //  1 0 9 8 7 6 5 4 3 2 1 0
   // [     shift    ][  Rm  ]

   // Get the shift type.
   shiftType = (instruction & 0x00000060);

   // Check shift type to get the shift value.
   if ((instruction & 0x00000010) > 0)
   {
      // Shift by amount in bottom byte of a register

      // TODO: Add prefetch logic?
      // NOTE: In this case, this operation causes prefetch to be 12 bytes instead of 8.

      regShift = (RegisterType) ((instruction & 0x00000F00) >> 8);
      shift = (*(REG->Reg (regShift))) & 0x000000FF;

      if (shift == 0)
      {
         // More documented special case processing. In this case, we force LSL 0!
         shiftType = 0;
      }
   }
   else
   {
      // Shift by a certain amount.
      shift = (instruction & 0x00000F80) >> 7;
   }
   
   // Get the value out of the register specified by Rm.
   op = *(REG->Reg ((RegisterType) (instruction & 0x0000000F)));

   switch (instruction & 0x00000060)
   {
   case 0x00000000:
      // 00 = logical left
      if (shift == 0)
      {
         // This is a documented special case in which C is preserved.
         *newCarry = REG-> CPSR()->GetCarry ();
         op <<= shift;
      }
      else if (shift == 32)
      {
         *newCarry = (op & 0x00000001);
         op = 0;
      }
      else if (shift > 32)
      {
         *newCarry = false;
         op = 0;
      }
      else
      {
         // the carry bit will be one of the previous bits in this operand. 
         *newCarry = (op & (uint) pow ((double) 2, 31 - shift + 1)) > 0;
         op <<= shift;
      }
      break;

   case 0x00000020:
      // 01 = logical right
      if (shift == 0 || shift == 32)
      {
         // This is a documented special case in which the shift is actually 32.
         *newCarry = (op & 0x80000000) > 0;
         op = 0;
      }
      else if (shift > 32)
      {
         *newCarry = false;
         op = 0;
      }
      else
      {
         // Regular logical right shift.
         *newCarry = (op & (uint) pow ((double) 2, shift - 1)) > 0;
         op >>= shift;
      }
      break;

   case 0x00000040:
      // 10 = arithmetic right
      topBit = (op & 0x80000000) > 0;
      if (shift == 0 || shift >= 32)
      {
         // This is a documented special case in which the shift is actually 32.
         *newCarry = topBit;
         op = topBit ? 0xFFFFFFFF : 0;
      }
      else
      {
         // Regular logical right shift.
         *newCarry = (op & (uint) pow ((double) 2, shift - 1)) > 0;
         op = op >> shift;

         // Now set all the bits that were shifted in as the highest previous bit!
         if (topBit)
         {
            op |= ~((long) (pow ((double) 2, 32 - shift) - 1));
         }
      }
      break;

   case 0x00000060:
      // 11 = rotate right
      
      if (shift > 32)
      {
         // Modulus this down to size.
         shift %= 32;
         if (shift == 0)
         {
            shift = 32; 
         }
      }
      
      if (shift == 0)
      {
         // This is a special "Rotate Right Extended" in which everything
         // is shifted right one bit including the carry bit.
         *newCarry = (op & 0x00000001) > 0;
         op = (op >> 1) & (REG->CPSR ()->GetCarry () ? 0x80000000 : 0);
      }
      else
      {
         // Normal rotate;
         *newCarry = (op & (uint) pow ((double) 2, shift - 1)) > 0;
         op = (op >> shift) | (op << (32 - shift));
      }
      break;
   }

   return op;
}

uint ARM60CPU::DoAdd (uint op1, uint op2, bool oldCarry, bool* newCarry)
{
   uint lowerSum;
   uint higherSum;
   
   lowerSum = (op1 & 0x0000FFFF) + (op2 & 0x0000FFFF) + (oldCarry ? 1 : 0);
   higherSum =  ((op1 & 0xFFFF0000) >> 16) + ((op2 & 0xFFFF0000) >> 16) + (lowerSum >> 16);
   
   *newCarry = (higherSum & 0xFFFF0000) > 1;
   
   return op1 + op2 + (oldCarry ? 1 : 0);
}