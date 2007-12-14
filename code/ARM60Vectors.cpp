#include "ARM60Vectors.h"

ARM60Vectors::ARM60Vectors ()
{
   m_vects = new uint [VECT_COUNT];

   // Potentially pointless initialization.
   for (int x = 0; x < VECT_COUNT; x++)
   {
      m_vects [x] = 0;
   }
}

ARM60Vectors::~ARM60Vectors ()
{
   delete m_vects;
}

uint* ARM60Vectors::Vector (VectorType vect)
{
   return &m_vects [vect];
}

