#ifndef _INC_ARM60CPU
#define _INC_ARM60CPU

#include "types.h"
#include "ARM60REGISTERS.h"

class ARM60CPU
{
public:
   ARM60CPU::ARM60CPU ();
   ARM60CPU::~ARM60CPU ();

private:
   ARM60Registers* m_reg;

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

   bool CheckCondition (uint instruction);
};

#endif // _INC_ARM60CPU