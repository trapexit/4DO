#include "ARM60Pipeline.h"

ARM60Pipeline::ARM60Pipeline (int maxItems)
{
   // Constructor
   m_maxItems = maxItems;
   m_items = new uint [m_maxItems];
   m_itemCount = 0;
}

ARM60Pipeline::~ARM60Pipeline ()
{
   // Destructor
   delete m_items;
}

void ARM60Pipeline::Clear ()
{
   m_itemCount = 0;
}

int ARM60Pipeline::Count ()
{
   return m_itemCount;
}

uint ARM60Pipeline::ReadLast ()
{
   if (m_itemCount > 0)
   {
      return m_items [m_itemCount - 1];
   }
   else
   {
      return 0;
   }
}

void ARM60Pipeline::Add (uint value)
{
   // Increment count.
   if (m_itemCount < m_maxItems)
   {
      m_itemCount++;
   }
   
   // Move all items.
   for (int i = m_maxItems - 2; i >= 0; i--)
   {
      m_items [i] = m_items [i - 1];
   }
}