#ifndef _INC_ARM60PIPELINE
#define _INC_ARM60PIPELINE

#include "types.h"

class ARM60Pipeline
{
public:
   ARM60Pipeline (int maxItems);
   ~ARM60Pipeline ();
   
   void Clear (); 
   int  Count ();
   uint ReadLast ();
   void Add (uint value);
   
private:
   uint* m_items;
   int   m_maxItems;
   int   m_itemCount;
};

#endif // _INC_ARM60PIPELINE