#ifndef _INC_ARM60CPU
#define _INC_ARM60CPU

#include <fstream>
#include <iostream>
#include <math.h>

#include "types.h"
#include "DMAController.h"
#include "BitMath.h"
#include "ARM60Register.h"
#include "ARM60Registers.h"
#include "ARM60Vectors.h"
#include "ARM60Pipeline.h"

class ARM60CPU
{
public:
   ARM60CPU ();
   ~ARM60CPU ();

   DMAController*  DMA;
   ARM60Registers* REG;
   
   bool BIGEND; // BIGEND is always true in the 3DO!
   bool LOCK;
   
   wxString LastResult;
   wxString LastCond;
   
   void DoSingleInstruction ();
   
private:
   void ProcessInstruction (uint instruction);
   void ProcessBranch (uint instruction);
   void ProcessDataProcessing (uint instruction);
   void ProcessPSRTransfer (uint instruction);
   void ProcessMultiply (uint instruction);
   void ProcessSingleDataTransfer (uint instruction);
   void ProcessBlockDataTransfer (uint instruction);
   void ProcessSingleDataSwap (uint instruction);
   void ProcessSoftwareInterrupt (uint instruction);
   void ProcessCoprocessorDataOperations (uint instruction);
   void ProcessCoprocessorDataTransfers (uint instruction);
   void ProcessCoprocessorRegisterTransfers (uint instruction);
   void ProcessUndefined (uint instruction);

   uint DoLDR (uint address, bool isByte);
   void DoSTR (uint address, RegisterType sourceReg, bool isByte);

   uint ReadShiftedRegisterOperand (uint instruction, bool* newCarry);
   uint DoAdd (uint op1, uint op2, bool oldCarry, bool* newCarry);
};

#endif // _INC_ARM60CPU