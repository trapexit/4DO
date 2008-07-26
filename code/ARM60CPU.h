#ifndef _INC_ARM60CPU
#define _INC_ARM60CPU

////////////////////
// Cycle definitions (will never change!)
#define NCYCLES 4 // Number of cycles needed for a N-cycle (Non-sequential cycle)
#define SCYCLES 1 // Number of cycles needed for a S-cycle (Sequential cycle)
#define ICYCLES 1 // Number of cycles needed for a I-cycle (Internal cycle)
#define CCYCLES 1 // Number of cycles needed for a C-cycle (Coprocessor register transfer cycle)
				  // NOTE: Co-processor access is never performed in the 3DO system.

#include <fstream>
#include <iostream>
#include <math.h>

#include "types.h"
#include "DMAController.h"
#include "BitMath.h"
#include "ARM60Register.h"
#include "ARM60Registers.h"

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
   
   void ExecuteCycles( uint cycles );
   void DoSingleInstruction ();
   
private:
   void ProcessInstruction					(uint instruction);
   void ProcessBranch						(uint instruction);
   void ProcessDataProcessing				(uint instruction);
   void ProcessPSRTransfer					(uint instruction);
   void ProcessMultiply						(uint instruction);
   void ProcessSingleDataTransfer			(uint instruction);
   void ProcessBlockDataTransfer			(uint instruction);
   void ProcessSingleDataSwap				(uint instruction);
   void ProcessSoftwareInterrupt			(uint instruction);
   void ProcessCoprocessorDataOperations	(uint instruction);
   void ProcessCoprocessorDataTransfers		(uint instruction);
   void ProcessCoprocessorRegisterTransfers (uint instruction);
   void ProcessUndefined					(uint instruction);

   uint DoLDR (uint address, bool isByte);
   void DoSTR (uint address, RegisterType sourceReg, bool isByte);

   uint ReadShiftedRegisterOperand (uint instruction, bool* newCarry);
   uint DoAdd (uint op1, uint op2, bool oldCarry, bool* newCarry);
   
   uint m_cycleCount;
};

#endif // _INC_ARM60CPU