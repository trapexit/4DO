#include "ARM60CPU.h"

ARM60CPU::ARM60CPU ()
{
   m_reg = new ARM60Registers ();

   // NOTES:
   // * All images scanned so far seem identical until BYTE 72 (a.k.a. 73)
   // * "iamaduck" seems to be a NO-OP in many images? ... What the hell?
   // * TODO: Double-check before starting this part, but I'm assuming big-endian.

   
   /*
   /////////
   
   ifstream romFile;
   unsigned char*    buffer;
   int      length = 200;
   int x;

   //romFile.open ("C:\\emulation\\3do\\ROMS\\Trip'd (1995)(Panasonic)(Eu-US)[!].iso");
   //romFile.open ("C:\\emulation\\3do\\ROMS\\Out of this World (1993)(Interplay)(US)[!][45097-1].iso");
   //romFile.open ("C:\\emulation\\3do\\ROMS\\Alone in the Dark (1994)(Interplay)(US)[!].iso");
   //romFile.open ("C:\\emulation\\3do\\ROMS\\Lost Eden (1993)(Virgin)(US).iso");
   //romFile.open ("C:\\Code\\unCD-ROM14\\alone\\LaunchMe");
   romFile.open ("C:\\Code\\unCD-ROM14\\alone\\playmovie");

   buffer = new unsigned char [length];
   romFile.read (buffer, length);

   for (x = 0; x < length; x++)
   {
      cout << x << "\tchar:\t" << (int) buffer [x] << "\t" << buffer [x] << "\t" << CharToBitString (buffer [x]) << endl;
   }

   cout << "---------------" << endl;

   for (x = 0; x < length; x+=4)
   {
      cout << x << "\t" << CharToBitString (buffer [x]) << " " << CharToBitString (buffer [x + 1]) << CharToBitString (buffer [x + 2]) << CharToBitString (buffer [x + 3]) << "  " << CharToBitString (buffer [x + 3]) << "  "<< CharToBitString (buffer [x + 2]) << CharToBitString (buffer [x + 1]) << CharToBitString (buffer [x]) << endl;
   }

   for (x = 0; x < length; x+=4)
   {
      this->ProcessInstruction((buffer[x] << 24) | (buffer[x + 1] << 16) | (buffer[x + 2] << 8) | buffer[x + 3]);
      //this->ProcessInstruction((buffer[x + 3] << 24) | (buffer[x + 2] << 16) | (buffer[x + 1] << 8) | buffer[x]);
   }

   int y = -15;
   uint y2;

   unsigned char y3;
   
   y3= y;
   y2 = (uint) y3;

   cout << y << "\t" << y2 << endl;
   
   romFile.close ();
   delete buffer;
   */
}

ARM60CPU::~ARM60CPU ()
{
   delete m_reg;
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

void ARM60CPU::ProcessBranch (uint instruction)
{
   // Check condition Field.
   if (! CheckCondition (instruction))
      return;

   ///////////////////////
   int  offset;

   if ((instruction & 0x01000000) == 0x01000000)
   {
      // TODO: Check on prefetch logic.
      *(m_reg->Reg (ARM60_R14)) = *(m_reg->PC ()->Value);
   }
   
   // Offset is bit shifted left two, then sign extended to 32 bits.
   offset = (instruction & 0x00FFFFFF) << 2;
   if ((offset & 0x02000000) > 0)
   {
      offset &= 0xFC000000;
   }
   (*(m_reg->PC ()->Value)) += offset;
}

void ARM60CPU::ProcessDataProcessing (uint instruction)
{
   //////////////////////
   // 10987654321098765432109876543210
   // Cond  I    S    [Rd]
   //     00 OpCd [Rn]    [ Operand 2]

   // Check condition Field.
   if (! CheckCondition (instruction))
      return;
   
   //////////////////////
   bool    newCarry = false;
   bool    setCond;
   bool    writeResult = true; // NOTE: The assembler allegegly always sets S to false here.
   bool    isLogicOp = true;
   uint    op1;
   uint    op2;
   uint    result = 0;
   __int64 resultLong = 0;
   int     opCode;
   int     regDest;
   int     shift;

   opCode = (instruction & 0x01E00000) >> 21;
   setCond = (instruction & 0x00100000) > 0;
   
   regDest = (instruction & 0x000F000) >> 12;

   ////////////////////////////
   // Get first operand value.
   op1 = *(m_reg->Reg ((RegisterType) (instruction & 0x000F0000)));
   
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
      resultLong = op1 & op2;
      return;
   
   case 0x1:
      // EOR
      resultLong = op1 ^ op2;
      return;
   
   case 0x2:
      // SUB
      isLogicOp = false;
      resultLong = op1 - op2;
      return;
   
   case 0x3:
      // RSB
      isLogicOp = false;
      resultLong = op2 - op1;
      return;
   
   case 0x4:
      // ADD
      isLogicOp = false;
      resultLong = op1 + op2;
      return;
   
   case 0x5:
      // ADC
      isLogicOp = false;
      resultLong = op1 + op2 + (m_reg->CPSR ()->GetCarry () ? 1 : 0);
      return;
   
   case 0x6:
      // SBC
      isLogicOp = false;
      resultLong = op1 - op2 + (m_reg->CPSR ()->GetCarry () ? 1 : 0) - 1;
      return;
   
   case 0x7:
      // RSC
      isLogicOp = false;
      resultLong = op2 - op1 + (m_reg->CPSR ()->GetCarry () ? 1 : 0) - 1;
      return;
   
   case 0x8:
      // TST
      writeResult = false;
      resultLong = op2 & op1;
      return;
   
   case 0x9:
      // TEQ
      writeResult = false;
      resultLong = op2 ^ op1;
      return;
   
   case 0xA:
      // CMP
      writeResult = false;
      isLogicOp = false;
      resultLong = op2 - op1;
      return;
   
   case 0xB:
      // CMN
      writeResult = false;
      isLogicOp = false;
      resultLong = op2 - op1;
      return;
   
   case 0xC:
      // ORR
      resultLong = op2 | op1;
      return;
   
   case 0xD:
      // MOV
      resultLong = op2;
      return;

   case 0xE:
      // BIC
      resultLong = op1 & (~op2);
      return;

   case 0xF:
      // MVN
      resultLong = ~op2;
      return;

   }

   result = (uint) resultLong;

   ///////////////////////
   // Write result if necessary.
   if (writeResult)
   {
      (*(m_reg->Reg ((RegisterType) regDest))) = result;
   }

   ///////////////////////
   // Write condition flags if instructed to.

   if (setCond)
   {
      if (regDest == (int) ARM60_PC)
      {
         if (m_reg->CPSR ()->GetCPUMode() == CPUMODE_USR)
         {
            // Hm, this shouldn't happen.
            // TODO: Error?
         }
         else
         {
            // SPSR of current mode is written to CPSR;
            *(m_reg->Reg (ARM60_CPSR)) = *(m_reg->Reg (ARM60_SPSR));
         }
      }
      
      if (isLogicOp)
      {
         // Overflow bit is not changed in this scenario.
         m_reg->CPSR ()->SetCarry (newCarry);
         m_reg->CPSR ()->SetZero (result == 0);
         m_reg->CPSR ()->SetNegative ((result & 0x80000000) > 0);
      }
      else
      {
         // TODO: Double-check carry and overflow logic!!
         
         // Overflow bit is specific to two's compliment operations.
         // It will be set if the sign of the two operands differs
         // from the sign of the result.
         if ((op1 & 0x80000000) == (op2 & 0x80000000))
         {
            m_reg->CPSR ()->SetOverflow (!
                  ((result & 0x80000000) == (op1 & 0x80000000)));
         }
         else
         {
            // Overflow is impossible.
            m_reg->CPSR ()->SetOverflow (false);
         }
         m_reg->CPSR ()->SetCarry ((resultLong & 0x100000000) > 0);
         m_reg->CPSR ()->SetZero (result == 0);
         m_reg->CPSR ()->SetNegative ((result & 0x80000000) > 0);
      }
   }
}

void ARM60CPU::ProcessPSRTransfer (uint instruction)
{
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
         *(m_reg->Reg ((RegisterType) ((instruction & 0x0000F000) > 12))) = 
            *(m_reg->Reg (ARM60_SPSR));
      }
      else
      {
         // Source is CPSR
         *(m_reg->Reg ((RegisterType) ((instruction & 0x0000F000) > 12))) = 
            *(m_reg->Reg (ARM60_CPSR));
      }
      break;

   case 0x00290000:
      // MSR - transfer register contents to PSR
      if ((instruction & 0x00400000) > 0)
      {
         // Destination is SPSR
         *(m_reg->Reg (ARM60_SPSR)) = 
            *(m_reg->Reg ((RegisterType) (instruction & 0x0000000F)));
      }
      else
      {
         // Destination is CPSR
         *(m_reg->Reg (ARM60_CPSR)) = 
            *(m_reg->Reg ((RegisterType) (instruction & 0x0000000F)));
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
         sourceVal = *(m_reg->Reg ((RegisterType) (instruction & 0x0000000F)));
      }
      
      //////////////
      // Give to destination.
      if ((instruction & 0x00400000) > 0)
      {
         // Destination is SPSR
         *(m_reg->Reg (ARM60_SPSR)) = sourceVal;
      }
      else
      {
         // Destination is CPSR
         *(m_reg->Reg (ARM60_CPSR)) = sourceVal;
      }
      break;
   }
}

void ARM60CPU::ProcessMultiply (uint instruction)
{
   // Check condition Field.
   if (! CheckCondition (instruction))
      return;

   //////////////////////
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

   result = (*(m_reg->Reg (Rm))) * (*(m_reg->Reg (Rs)));

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
      result += *(m_reg->Reg (Rn));
   }

   // Set the result to Rd
   *(m_reg->Reg ((RegisterType) (instruction & 0x000F0000))) = result;
   
   // Optionally set CPSR values.
   if ((instruction & 0x00100000) > 0)
   {
      // Set CPSR values.
      
      // NOTE: Overflow is unaffected.

      // NOTE: Carry is set to a "meaningless value".
      m_reg->CPSR ()->SetCarry (0);
      m_reg->CPSR ()->SetZero (result == 0);
      m_reg->CPSR ()->SetNegative ((result & 0x80000000) > 0);
   }
}

void ARM60CPU::ProcessSingleDataTransfer (uint instruction)
{
   // Check condition Field.
   if (! CheckCondition (instruction))
      return;

   //////////////////////
   // 10987654321098765432109876543210
   // Cond  I U W [Rn]    [  Offset  ]
   //     01 P B L    [Rd]
   //            (base)
   //                (dest)
   ///////////////////////////

   bool  newCarry = false;
   int   offset;
   uint  value;
   uint* baseReg;
   uint* destReg;
   uint  address;

   ////////////////////////////////////////////////////////

   baseReg = m_reg->Reg ((RegisterType) (0x000F0000 >> 16));
   destReg = m_reg->Reg ((RegisterType) (0x0000F000 >> 12));

   // TODO: Special cases around use of R15 (prefetch).

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
      // Load from memory
      value = DMA->GetValue (address);
      
      if ((instruction & 0x00400000) > 0)
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
         *(destReg) = value;
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
         *(destReg) = value;
      }
   }
   else
   {
      // Store to memory
      if ((instruction & 0x00400000) > 0)
      {
         // Storing a byte...
         // We store the bottom byte repeated four times.
         // TODO: Is this supposed be be affected by big endian vs little?
         value = *(destReg) & 0x000000FF;
         value = value | (value << 8) | (value << 16) | (value << 24);
         DMA->SetValue (address, value);
      }
      else
      {
         // Storing a word. Easy.
         DMA->SetValue (address, *(destReg));
      }
   }
}

void ARM60CPU::ProcessBlockDataTransfer (uint instruction)
{
   // Check condition Field.
   if (! CheckCondition (instruction))
      return;
}

void ARM60CPU::ProcessSingleDataSwap (uint instruction)
{
   // Check condition Field.
   if (! CheckCondition (instruction))
      return;
}

void ARM60CPU::ProcessSoftwareInterrupt (uint instruction)
{
   // Check condition Field.
   if (! CheckCondition (instruction))
      return;
}

void ARM60CPU::ProcessCoprocessorDataOperations (uint instruction)
{
   // Check condition Field.
   if (! CheckCondition (instruction))
      return;
}

void ARM60CPU::ProcessCoprocessorDataTransfers (uint instruction)
{
   // Check condition Field.
   if (! CheckCondition (instruction))
      return;
}

void ARM60CPU::ProcessCoprocessorRegisterTransfers (uint instruction)
{
   // Check condition Field.
   if (! CheckCondition (instruction))
      return;
}

void ARM60CPU::ProcessUndefined (uint instruction)
{
   // Check condition Field.
   if (! CheckCondition (instruction))
      return;
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
      shift = (*(m_reg->Reg (regShift))) & 0x000000FF;

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
   op = *(m_reg->Reg ((RegisterType) (instruction & 0x0000000F)));

   switch (instruction & 0x00000060)
   {
   case 0x00000000:
      // 00 = logical left
      if (shift == 0)
      {
         // This is a documented special case in which C is preserved.
         *newCarry = m_reg-> CPSR()->GetCarry ();
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
         *newCarry = (op & (uint) pow (2, 31 - shift + 1)) > 0;
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
         *newCarry = (op & (uint) pow (2, shift - 1)) > 0;
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
         *newCarry = (op & (uint) pow (2, shift - 1)) > 0;
         op = op >> shift;

         // Now set all the bits that were shifted in as the highest previous bit!
         if (topBit)
         {
            op |= ~((long) (pow (2, 32 - shift) - 1));
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
         op = (op >> 1) & (m_reg->CPSR ()->GetCarry () ? 0x80000000 : 0);
      }
      else
      {
         // Normal rotate;
         *newCarry = (op & (uint) pow (2, shift - 1)) > 0;
         op = (op >> shift) | (op << (32 - shift));
      }
      break;
   }

   return op;
}
