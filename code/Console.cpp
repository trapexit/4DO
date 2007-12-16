#include "Console.h"

Console::Console ()
{
   // Constructor
   m_DMA = new DMAController ();
   
   m_CPU = new ARM60CPU ();
   m_CPU->DMA = m_DMA;
   m_CPU->BIGEND = true; // Big endian is always true!
}

ARM60CPU* Console::CPU ()
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