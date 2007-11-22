#include "ARM60Register.h"

class ARM60PSRegister : public ARM60Register
{
public:
   ARM60PSRegister::ARM60PSRegister (uint* value);
   
   bool GetNegative ();
   bool GetZero ();
   bool GetCarry ();
   bool GetOverflow ();
   bool GetFIQDisable ();
   bool GetIRQDisable ();
};

/*
   public bool Negative 
   {
      get 
      {
         return BitMath.GetBit (this.Value, 31);
      }
      set
      {
         this.Value = BitMath.SetBit (this.Value, 31, value);
      }
   }

   public bool Zero 
   {
      get 
      {
         return BitMath.GetBit (this.Value, 30);
      }
      set
      {
         this.Value = BitMath.SetBit (this.Value, 30, value);
      }
   }
   
   public bool Carry 
   {
      get 
      {
         return BitMath.GetBit (this.Value, 29);
      }
      set
      {
         this.Value = BitMath.SetBit (this.Value, 29, value);
      }
   }
   
   public bool Overflow 
   {
      get 
      {
         return BitMath.GetBit (this.Value, 28);
      }
      set
      {
         this.Value = BitMath.SetBit (this.Value, 28, value);
      }
   }
   
   public bool FIQDisable 
   {
      get 
      {
         return BitMath.GetBit (this.Value, 6);
      }
      set
      {
         this.Value = BitMath.SetBit (this.Value, 6, value);
      }
   }
   
   public bool IRQDisable 
   {
      get 
      {
         return BitMath.GetBit (this.Value, 7);
      }
      set
      {
         this.Value = BitMath.SetBit (this.Value, 7, value);
      }
   }
   
   public CPUMode Mode
   {
      get
      {
         uint mode;
         mode = this.Value & 0x1F;
         
         switch (mode)
         {
         case 0x10:
            return CPUMode.USR;
         
         case 0x11:
            return CPUMode.FIQ;
         
         case 0x12:
            return CPUMode.IRQ;
         
         case 0x13:
            return CPUMode.SVC;
         
         case 0x17:
            return CPUMode.ABT;
         
         case 0x1B:
            return CPUMode.UND;
         
         }
         
         // NOTE: I'm unsure if this will ever happen, but I don't want it to crash.
         return CPUMode.Invalid;
      }
      set
      {
         switch (value)
         {
         case CPUMode.USR:
            BitMath.SetBits (this.Value, 0x0000001F, 0x10);
            break;
         
         case CPUMode.FIQ:
            BitMath.SetBits (this.Value, 0x0000001F, 0x11);
            break;
         
         case CPUMode.IRQ:
            BitMath.SetBits (this.Value, 0x0000001F, 0x12);
            break;
         
         case CPUMode.SVC:
            BitMath.SetBits (this.Value, 0x0000001F, 0x13);
            break;
         
         case CPUMode.ABT:
            BitMath.SetBits (this.Value, 0x0000001F, 0x17);
            break;
         
         case CPUMode.UND:
            BitMath.SetBits (this.Value, 0x0000001F, 0x1B);
            break;
         }
      }   
   }
*/
