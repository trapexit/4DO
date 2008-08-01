#include "Console.h"

Console::Console ()
{
   // Constructor
   m_DMA = new DMAController ();
   
   m_CPU = new ARMCPU ();
   m_CPU->DMA = m_DMA;
   
   // Set to user mode;
   m_CPU->SetCPSR(0x00000010);
}

ARMCPU* Console::CPU ()
{
   return m_CPU;
}

DMAController* Console::DMA ()
{
   return m_DMA;
}

Console::~Console ()
{
   // Destructor
   delete m_DMA;
   delete m_CPU;
}