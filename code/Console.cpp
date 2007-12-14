#include "Console.h"

Console::Console ()
{
   // Constructor
   m_DMA = new DMAController ();
   
   m_CPU = new ARM60CPU ();
   m_CPU->dma = m_DMA;
}

Console::~Console ()
{
   // Destructor
   delete m_DMA;
   delete m_CPU;
}

