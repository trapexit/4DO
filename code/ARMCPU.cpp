#include "ARMCPU.h"
#include "string.h"

// ARM Instruction masks
#define ARM_MUL_MASK    0x0fc000f0
#define ARM_MUL_SIGN    0x00000090
#define ARM_SDS_MASK    0x0fb00ff0
#define ARM_SDS_SIGN    0x01000090
#define ARM_UND_MASK    0x0e000010
#define ARM_UND_SIGN    0x06000010
#define ARM_MRS_MASK    0x0fbf0fff
#define ARM_MRS_SIGN    0x010f0000
#define ARM_MSR_MASK    0x0fbffff0
#define ARM_MSR_SIGN    0x0129f000
#define ARM_MSRF_MASK   0x0dbff000
#define ARM_MSRF_SIGN   0x0128f000

// ARM mode (arbitrary numbers)
#define ARM_MODE_USER   0
#define ARM_MODE_FIQ    1
#define ARM_MODE_IRQ    2
#define ARM_MODE_SVC    3
#define ARM_MODE_ABT    4
#define ARM_MODE_UND    5
#define ARM_MODE_UNK    0xff

// ARM Mode Codes (the values stored in PSR, and used to fetch from arm_mode_table)
#define ARM_MODE_CODE_USER   0x10
#define ARM_MODE_CODE_FIQ    0x11
#define ARM_MODE_CODE_IRQ    0x12
#define ARM_MODE_CODE_SVC    0x13
#define ARM_MODE_CODE_ABT    0x17
#define ARM_MODE_CODE_UND    0x1b

// Definition of the relative cycle lengths
#define NCYCLE 4
#define SCYCLE 1
#define ICYCLE 1

const static uint8 arm_mode_table[]=
{
     ARM_MODE_UNK,     ARM_MODE_UNK,     ARM_MODE_UNK,     ARM_MODE_UNK,
     ARM_MODE_UNK,     ARM_MODE_UNK,     ARM_MODE_UNK,     ARM_MODE_UNK,
     ARM_MODE_UNK,     ARM_MODE_UNK,     ARM_MODE_UNK,     ARM_MODE_UNK,
     ARM_MODE_UNK,     ARM_MODE_UNK,     ARM_MODE_UNK,     ARM_MODE_UNK,
     ARM_MODE_USER,    ARM_MODE_FIQ,     ARM_MODE_IRQ,     ARM_MODE_SVC,
     ARM_MODE_UNK,     ARM_MODE_UNK,     ARM_MODE_UNK,     ARM_MODE_ABT,
     ARM_MODE_UNK,     ARM_MODE_UNK,     ARM_MODE_UNK,     ARM_MODE_UND,
     ARM_MODE_UNK,     ARM_MODE_UNK,     ARM_MODE_UNK,     ARM_MODE_UNK
};

//The following is a condition table used to quickly check the condition
//field of the instruction against the CPSR.
//
//Most of these checks are simple, but some have multiple possible CPSR
//values that result success. To handle this, the table values below
//are shifted to the right by the current value in the CPSR, and the
//result is AND compared to the number "1". This allows multiple comparisons
//to be done in one operation.
//
// NOTE: The documentation specifies that the absence of a condition
//       code acts as though "always" had been specified, although
//       I don't see how this is possible.
const static uint16 cond_flags_cross[]={   //((cond_flags_cross[cond_feald]>>flags)&1)  -- пример проверки
    0xf0f0, //EQ - Z set (equal)
    0x0f0f, //NE - Z clear (not equal)
    0xcccc, //CS - C set (unsigned higher or same)
    0x3333, //CC - C clear (unsigned lower)
    0xff00, //N set (negative)
    0x00ff, //N clear (positive or zero)
    0xaaaa, //V set (overflow)
    0x5555, //V clear (no overflow)
    0x0c0c, //C set and Z clear (unsigned higher)
    0xf3f3, //C clear or Z set (unsigned lower or same)
    0xaa55, //N set and V set, or N clear and V clear (greater or equal)
    0x55aa, //N set and V clear, or N clear and V set (less than)
    0x0a05, //Z clear, and either N set and V set, or N clear and V clear (greater than)
    0xf5fa, //Z set, or N set and V clear, or N clear and V set (less than or equal)
    0xffff, //always
    0x0000  //never
};

static bool nvram_autosave=true;

#define REG_PC				RON_USER[15]
#define UNDEFVAL			0xBAD12345

#define CYCLES				ARM.CYCLES
#define MAS_Access_Exept	ARM.MAS_Access_Exept
#define pRam				ARM.Ram
#define pRom				ARM.Rom
#define pNVRam				ARM.NVRam
#define RON_USER			ARM.USER	
#define RON_CASH			ARM.CASH
#define RON_SVC				ARM.SVC
#define RON_ABT				ARM.ABT
#define RON_FIQ				ARM.FIQ
#define RON_IRQ				ARM.IRQ
#define RON_UND				ARM.UND
#define SPSR				ARM.SPSR
#define CPSR				ARM.CPSR
#define gFIQ				ARM.nFIQ //внешнее прерывание, устанавливается другими процессорами
#define gSecondROM			ARM.SecondROM	//селектор ПЗУ (рум и шрифт)

#define ISN  ((CPSR>>31)&1)
#define ISZ  ((CPSR>>30)&1)
#define ISC  ((CPSR>>29)&1)
#define ISV  ((CPSR>>28)&1)

#define MODE ((CPSR&0x1f))
#define ISI	 ((CPSR>>7)&1)
#define ISF  ((CPSR>>6)&1)

#define ARM_SET_C(x)    (CPSR=((CPSR&0xdfffffff)|(((x)&1)<<29)))
#define ARM_SET_Z(x)    (CPSR=((CPSR&0xbfffffff)|((x)==0?0x40000000:0)))
#define ARM_SET_N(x)    (CPSR=((CPSR&0x7fffffff)|((x)&0x80000000)))
#define ARM_GET_C       ((CPSR>>29)&1)

__inline uint32 _bswap(uint32 x)
{
return (x>>24) | ((x>>8)&0x0000FF00L) | ((x&0x0000FF00L)<<8) | (x<<24);
}

/////////////////////////////////////////////////////////////////////////////////////
////////////////////          ARM Class Implementation            ///////////////////
/////////////////////////////////////////////////////////////////////////////////////

ARMCPU::ARMCPU ()
{
Reset();
}

ARMCPU::~ARMCPU ()
{
}

void __fastcall ARMCPU::SetFIQ( void )
{
gFIQ=true;
}

unsigned int __fastcall ARMCPU::GetFIQ()
{
return gFIQ;
}

void ARMCPU::SetPC( unsigned int addr )
{
REG_PC=addr;
}

void ARMCPU::Reset()
{
int i;
gSecondROM=0;
CYCLES=0;
for(i=0;i<16;i++)
    RON_USER[i]=0;
for(i=0;i<2;i++)
{
    RON_SVC[i]=0;
    RON_ABT[i]=0;
    RON_IRQ[i]=0;
    RON_UND[i]=0;
}
for(i=0;i<7;i++)
    RON_CASH[i]=RON_FIQ[i]=0;

MAS_Access_Exept=false;

REG_PC=0x00000000;
SetCPSR(ARM_MODE_CODE_SVC); //set svc mode
gFIQ=false;		//no FIQ!!!
gSecondROM=0;


//_clio_Reset();
//_madam_Reset();
}

#pragma region Simple Binary Operations

unsigned int __fastcall ARMCPU::calcbits(unsigned int num)
{
  __asm{         //calc bits - code is not portable!!!!!!!!!!!! x86 only!!!!!!!
        mov     eax,num
        bsr     eax,eax
        mov		num,eax
 }
 return num;
}

__inline uint32 ARMCPU::SHIFT_NSC(uint32 value, uint8 shift, uint8 type)
{
  switch(type)
  {
   case 0:
	    if(shift==0)return value;
		if(shift>31)return 0;
        return value<<shift;
   case 1:
	    if(shift==0)return value;
		if(shift>31)return 0;
        return value>>shift;
   case 2:
	    if(shift==0)return value;
		if(shift>31)return (((signed int)value)>>31);
        return (((signed int)value)>>shift);
   case 3:
        shift&=31;
		if(shift==0)return value;
        return ROTR(value, shift);
   case 4:
        return (value>>1)|(ARM_GET_C<<31);
  }
  return 0;
}

__inline uint32 ARMCPU::SHIFT_SC(uint32 value, uint8 shift, uint8 type)
{
uint32 tmp;
  switch(type)
  {
   case 0:
        if(shift)
		{
			if(shift>32) ARM_SET_C(0);
			else ARM_SET_C(((value<<(shift-1))&0x80000000)>>31);
		}
		if(shift==0)return value;
		if(shift>31)return 0;
        return value<<shift;
   case 1:
        if(shift)
		{
			if(shift>32) ARM_SET_C(0);
			else ARM_SET_C((value>>(shift-1))&1);
		}
		if(shift==0)return value;
		if(shift>31)return 0;
        return value>>shift;
   case 2:
        if(shift)
		{
			if(shift>32) ARM_SET_C((((signed int)value)>>31)&1); 
			else ARM_SET_C((((signed int)value)>>(shift-1))&1);		
		}
		if(shift==0)return value;
		if(shift>31)return (((signed int)value)>>31);
		return ((signed int)value)>>shift;
   case 3:
		if(shift)
        {
                shift=((shift)&31);
				if(shift)
				{
					ARM_SET_C((value>>(shift-1))&1);
				}
				else
				{
					ARM_SET_C((value>>31)&1);
				}
        }
		if(shift==0)return value;
        return ROTR(value, shift);
   case 4:
 	    tmp=ARM_GET_C<<31;
	    ARM_SET_C(value&1);
        return (value>>1)|(tmp);
  }

  return 0;
}

__inline uint32 ARMCPU::ROTR(uint32 op, uint32 val)
{
	// Rotate value.
	return (op >> val) | (op << (32 - val));	
}

__inline bool ARMCPU::SWAP(uint32 cmd)
{
	
    unsigned int tmp, addr;
        
    addr=RON_USER[(cmd>>16)&0xf]+((((cmd>>16)&0xf)==0xf)?4:0);

    if(cmd&(1<<22))
    {
        tmp=mreadb(addr);
		if(MAS_Access_Exept)return true;
        mwriteb(addr, RON_USER[cmd&0xf]+(((cmd&0xf)==0xf)?8:0));
		if(MAS_Access_Exept)return true;
        RON_USER[(cmd>>12)&0xf]=tmp;
    }
    else
    {
        tmp=mreadw(addr);
        if(MAS_Access_Exept)return true;
        mwritew(addr, RON_USER[cmd&0xf]+(((cmd&0xf)==0xf)?8:0));
		if(MAS_Access_Exept)return true;
		if(addr&3)tmp=(tmp>>((addr&3)<<3))|(tmp<<(32-((addr&3)<<3)));
        RON_USER[(cmd>>12)&0xf]=tmp;
    }
	return false;
}

__inline bool ARMCPU::ALU_Exec(uint32 inst, uint8 opc, uint32 op1, uint32 op2, uint32 *Rd)
{
switch(opc)
{
case 0:
    *Rd=op1&op2;
    break;
case 2:
    *Rd=op1^op2;
    break;
case 4:
    *Rd=op1-op2;
    break;
case 6:
    *Rd=op2-op1;
    break;
case 8:
    *Rd=op1+op2;
    break;
case 10:
    *Rd=op1+op2+ARM_GET_C;
    break;
case 12:
    *Rd=op1-op2-(ARM_GET_C^1);
    break;
case 14:
    *Rd=op2-op1-(ARM_GET_C^1);
    break;
case 16:
case 20:	    
	if((inst>>22)&1)
		RON_USER[(inst>>12)&0xf]=SPSR[arm_mode_table[CPSR&0x1f]];
	else
		RON_USER[(inst>>12)&0xf]=CPSR;

	return true;
case 18:   
case 22:       
	if(!((inst>>16)&0x1) || !(arm_mode_table[MODE]))
	{		
		if((inst>>22)&1)
			SPSR[arm_mode_table[MODE]]=(SPSR[arm_mode_table[MODE]]&0x0fffffff)|(op2&0xf0000000);
		else
			CPSR=(CPSR&0x0fffffff)|(op2&0xf0000000);
	}
	else
	{
		if((inst>>22)&1)
			SPSR[arm_mode_table[MODE]]=op2&0xf00000df;
		else
			SetCPSR(op2);
	}
    return true;        
case 24:
	*Rd=op1|op2;
	break;
case 26:
    *Rd=op2;
    break;
case 28:
    *Rd=op1&(~op2);
    break;
case 30:
    *Rd=~op2;
    break;
case 1:
    *Rd=op1&op2;
    ARM_SET_Z(*Rd);
    ARM_SET_N(*Rd);
    break;
case 3:
    *Rd=op1^op2;
    ARM_SET_Z(*Rd);
    ARM_SET_N(*Rd);
    break;
case 5:
    *Rd=op1-op2;
    ARM_SET_Z(*Rd);
    ARM_SET_N(*Rd);
    SET_CV_sub(*Rd,op1,op2);
    break;
case 7:
    *Rd=op2-op1;
    ARM_SET_Z(*Rd);
    ARM_SET_N(*Rd);
    SET_CV_sub(*Rd,op2,op1);
    break;
case 9:
    *Rd=op1+op2;
    ARM_SET_Z(*Rd);
    ARM_SET_N(*Rd);
    SET_CV(*Rd,op1,op2);
    break;

case 11:
	*Rd=op1+op2+ARM_GET_C;
    ARM_SET_Z(*Rd);
    ARM_SET_N(*Rd);
    SET_CV(*Rd,op1,op2);
    break;
case 13: 
    *Rd=op1-op2-(ARM_GET_C^1);
    ARM_SET_Z(*Rd);
    ARM_SET_N(*Rd);        
	SET_CV_sub(*Rd,op1,op2);
    break;
case 15:
    *Rd=op2-op1-(ARM_GET_C^1);
    ARM_SET_Z(*Rd);
    ARM_SET_N(*Rd);
    SET_CV_sub(*Rd,op2,op1);
    break;//*/
case 17:
    op1&=op2;
    ARM_SET_Z(op1);
    ARM_SET_N(op1);
	return true;        
case 19:
    op1^=op2;
    ARM_SET_Z(op1);
    ARM_SET_N(op1);
    return true;
case 21:
    SET_CV_sub(op1-op2,op1,op2);
    ARM_SET_Z(op1-op2);
    ARM_SET_N(op1-op2);
    return true;
case 23:
    SET_CV(op1+op2,op1,op2);
    ARM_SET_Z(op1+op2);
    ARM_SET_N(op1+op2);
    return true;
case 25:
    *Rd=op1|op2;
    ARM_SET_Z(*Rd);
    ARM_SET_N(*Rd);
    break;
case 27:
    *Rd=op2;
    ARM_SET_Z(*Rd);
    ARM_SET_N(*Rd);
    break;
case 29:
    *Rd=op1&(~op2);
    ARM_SET_Z(*Rd);
    ARM_SET_N(*Rd);
    break;
case 31:
    *Rd=~op2;
    ARM_SET_Z(*Rd);
    ARM_SET_N(*Rd);
    break;
};
return false;
}

__inline void ARMCPU::SET_CV(uint32 rd, uint32 op1, uint32 op2)
{
  	CPSR=(CPSR&0xcfffffff)|
		((((op1 & op2) | ((~rd) & (op1 | op2)))&0x80000000)>>2) |
		(((((op1&(op2&(~rd))) | ((~op1)&(~op2)&rd)))&0x80000000)>>3);	

}

__inline void ARMCPU::SET_CV_sub(uint32 rd, uint32 op1, uint32 op2)
{
	CPSR=(CPSR&0xcfffffff)|
		(( ( ~( ((~op1) & op2) | (rd&((~op1)|op2))) )&0x80000000)>>2) |
		(((((op1&((~op2)&(~rd))) | ((~op1)&op2&rd)))&0x80000000)>>3);
}

// This functions d'nt change mode bits, then need no update regcur
__inline void ARMCPU::SETN(bool a) { CPSR=(CPSR&0x7fffffff)|((a?1:0)<<31); }
__inline void ARMCPU::SETZ(bool a) { CPSR=(CPSR&0xbfffffff)|((a?1:0)<<30); }
__inline void ARMCPU::SETC(bool a) { CPSR=(CPSR&0xdfffffff)|((a?1:0)<<29); }
__inline void ARMCPU::SETV(bool a) { CPSR=(CPSR&0xefffffff)|((a?1:0)<<28); }
__inline void ARMCPU::SETI(bool a) { CPSR=(CPSR&0xffffff7f)|((a?1:0)<<7); }
__inline void ARMCPU::SETF(bool a) { CPSR=(CPSR&0xffffffbf)|((a?1:0)<<6); }
__inline void ARMCPU::SETM(unsigned int a)
{
	if(arm_mode_table[a&0x1f]==ARM_MODE_UNK)
	{
		//!!Exeption!!
	}
	a|=0x10;
	Change_ModeSafe(a);
	CPSR=(CPSR&0xffffffe0)|(a & 0x1F);	
}

void ARMCPU::SetCPSR(unsigned int a)
{
if( arm_mode_table[ a&0x1f ]==ARM_MODE_UNK )
	{
	//!!Exeption!!
	}
a|=0x10;
Change_ModeSafe(a);
CPSR=a&0xf00000df;
}

#pragma endregion

#pragma region Mode Changing

void __fastcall ARMCPU::Change_ModeSafe(uint32 mode)
{
switch( arm_mode_table[ mode&0x1f ] )
{
case ARM_MODE_USER:
	RestUserRONS();
	break;
case ARM_MODE_FIQ:
	RestFiqRONS();
	break;
case ARM_MODE_IRQ:
	RestIrqRONS();
	break;
case ARM_MODE_SVC:
	RestSvcRONS();
	break;
case ARM_MODE_ABT:
	RestAbtRONS();
	break;
case ARM_MODE_UND:
	RestUndRONS();
	break;
}
}

__inline void ARMCPU::RestUserRONS()
{
switch(arm_mode_table[(CPSR&0x1f)])
{
case ARM_MODE_USER:
	break;
case ARM_MODE_FIQ:
	memcpy(RON_FIQ,&RON_USER[8],7<<2);
	memcpy(&RON_USER[8],RON_CASH,7<<2);
	break;
case ARM_MODE_IRQ:             
	RON_IRQ[0]=RON_USER[13];
	RON_IRQ[1]=RON_USER[14];
	RON_USER[13]=RON_CASH[5];
	RON_USER[14]=RON_CASH[6];             
	break;
case ARM_MODE_SVC:
	RON_SVC[0]=RON_USER[13];
	RON_SVC[1]=RON_USER[14];
	RON_USER[13]=RON_CASH[5];
	RON_USER[14]=RON_CASH[6];             
	break;
case ARM_MODE_ABT:
	RON_ABT[0]=RON_USER[13];
	RON_ABT[1]=RON_USER[14];
	RON_USER[13]=RON_CASH[5];
	RON_USER[14]=RON_CASH[6];   
	break;
case ARM_MODE_UND:
	RON_UND[0]=RON_USER[13];
	RON_UND[1]=RON_USER[14];
	RON_USER[13]=RON_CASH[5];
	RON_USER[14]=RON_CASH[6];
	break;
}
}

__inline void ARMCPU::RestFiqRONS()
{
switch(arm_mode_table[(CPSR&0x1f)])
{
case ARM_MODE_USER:
	memcpy(RON_CASH,&RON_USER[8],7<<2);
	memcpy(&RON_USER[8],RON_FIQ,7<<2);
	break;
case ARM_MODE_FIQ:
	break;
case ARM_MODE_IRQ:
	memcpy(RON_CASH,&RON_USER[8],5<<2);
	RON_IRQ[0]=RON_USER[13];
	RON_IRQ[1]=RON_USER[14];             
	memcpy(&RON_USER[8],RON_FIQ,7<<2);
	break;
case ARM_MODE_SVC:
	memcpy(RON_CASH,&RON_USER[8],5<<2);
	RON_SVC[0]=RON_USER[13];
	RON_SVC[1]=RON_USER[14];  
	memcpy(&RON_USER[8],RON_FIQ,7<<2);
	break;
case ARM_MODE_ABT:
	memcpy(RON_CASH,&RON_USER[8],5<<2);
	RON_ABT[0]=RON_USER[13];
	RON_ABT[1]=RON_USER[14];  
	memcpy(&RON_USER[8],RON_FIQ,7<<2);
	break;
case ARM_MODE_UND:
	memcpy(RON_CASH,&RON_USER[8],5<<2);
	RON_UND[0]=RON_USER[13];
	RON_UND[1]=RON_USER[14];  
	memcpy(&RON_USER[8],RON_FIQ,7<<2);
	break;
}
}

__inline void ARMCPU::RestIrqRONS()
{
switch(arm_mode_table[(CPSR&0x1f)])
{
case ARM_MODE_USER:
	RON_CASH[5]=RON_USER[13];
	RON_CASH[6]=RON_USER[14];
	RON_USER[13]=RON_IRQ[0];
	RON_USER[14]=RON_IRQ[1];             
	break;
case ARM_MODE_FIQ:
	memcpy(RON_FIQ,&RON_USER[8],7<<2);
	memcpy(&RON_USER[8],RON_CASH,5<<2);
	RON_USER[13]=RON_IRQ[0];
	RON_USER[14]=RON_IRQ[1];             
	break;
case ARM_MODE_IRQ:
	break;
case ARM_MODE_SVC:
	RON_SVC[0]=RON_USER[13];
	RON_SVC[1]=RON_USER[14];
	RON_USER[13]=RON_IRQ[0];
	RON_USER[14]=RON_IRQ[1];
	break;
case ARM_MODE_ABT:
	RON_ABT[0]=RON_USER[13];
	RON_ABT[1]=RON_USER[14];
	RON_USER[13]=RON_IRQ[0];
	RON_USER[14]=RON_IRQ[1];
	break;
case ARM_MODE_UND:
	RON_UND[0]=RON_USER[13];
	RON_UND[1]=RON_USER[14];
	RON_USER[13]=RON_IRQ[0];
	RON_USER[14]=RON_IRQ[1];
	break;
}
}

__inline void ARMCPU::RestSvcRONS()
{
switch(arm_mode_table[(CPSR&0x1f)])
{
case ARM_MODE_USER:
	RON_CASH[5]=RON_USER[13];
	RON_CASH[6]=RON_USER[14];
	RON_USER[13]=RON_SVC[0];
	RON_USER[14]=RON_SVC[1];             
	break;
case ARM_MODE_FIQ:
	memcpy(RON_FIQ,&RON_USER[8],7<<2);
	memcpy(&RON_USER[8],RON_CASH,5<<2);
	RON_USER[13]=RON_SVC[0];
	RON_USER[14]=RON_SVC[1]; 
	break;
case ARM_MODE_IRQ:
	RON_IRQ[0]=RON_USER[13];
	RON_IRQ[1]=RON_USER[14];
	RON_USER[13]=RON_SVC[0];
	RON_USER[14]=RON_SVC[1]; 
	break;
case ARM_MODE_SVC:
	break;
case ARM_MODE_ABT:
	RON_ABT[0]=RON_USER[13];
	RON_ABT[1]=RON_USER[14];
	RON_USER[13]=RON_SVC[0];
	RON_USER[14]=RON_SVC[1];
	break;
case ARM_MODE_UND:
	RON_UND[0]=RON_USER[13];
	RON_UND[1]=RON_USER[14];
	RON_USER[13]=RON_SVC[0];
	RON_USER[14]=RON_SVC[1];
	break;
}
}

__inline void ARMCPU::RestAbtRONS()
{
switch(arm_mode_table[(CPSR&0x1f)])
{
case ARM_MODE_USER:
	RON_CASH[5]=RON_USER[13];
	RON_CASH[6]=RON_USER[14];
	RON_USER[13]=RON_ABT[0];
	RON_USER[14]=RON_ABT[1]; 
	break;
case ARM_MODE_FIQ:
	memcpy(RON_FIQ,&RON_USER[8],7<<2);
	memcpy(&RON_USER[8],RON_CASH,5<<2);
	RON_USER[13]=RON_ABT[0];
	RON_USER[14]=RON_ABT[1]; 
	break;
case ARM_MODE_IRQ:
	RON_IRQ[0]=RON_USER[13];
	RON_IRQ[1]=RON_USER[14];
	RON_USER[13]=RON_ABT[0];
	RON_USER[14]=RON_ABT[1]; 
	break;
case ARM_MODE_SVC:
	RON_SVC[0]=RON_USER[13];
	RON_SVC[1]=RON_USER[14];
	RON_USER[13]=RON_ABT[0];
	RON_USER[14]=RON_ABT[1]; 
	break;
case ARM_MODE_ABT:
	break;
case ARM_MODE_UND:
	RON_UND[0]=RON_USER[13];
	RON_UND[1]=RON_USER[14];
	RON_USER[13]=RON_ABT[0];
	RON_USER[14]=RON_ABT[1]; 
	break;
}
}

__inline void ARMCPU::RestUndRONS()
{
switch(arm_mode_table[(CPSR&0x1f)])
{
case ARM_MODE_USER:
	RON_CASH[5]=RON_USER[13];
	RON_CASH[6]=RON_USER[14];
	RON_USER[13]=RON_UND[0];
	RON_USER[14]=RON_UND[1]; 
	break;
case ARM_MODE_FIQ:
	memcpy(RON_FIQ,&RON_USER[8],7<<2);
	memcpy(&RON_USER[8],RON_CASH,5<<2);
	RON_USER[13]=RON_UND[0];
	RON_USER[14]=RON_UND[1]; 
	break;
case ARM_MODE_IRQ:
	RON_IRQ[0]=RON_USER[13];
	RON_IRQ[1]=RON_USER[14];
	RON_USER[13]=RON_UND[0];
	RON_USER[14]=RON_UND[1]; 
	break;
case ARM_MODE_SVC:
	RON_SVC[0]=RON_USER[13];
	RON_SVC[1]=RON_USER[14];
	RON_USER[13]=RON_UND[0];
	RON_USER[14]=RON_UND[1];
	break;
case ARM_MODE_ABT:
	RON_ABT[0]=RON_USER[13];
	RON_ABT[1]=RON_USER[14];
	RON_USER[13]=RON_UND[0];
	RON_USER[14]=RON_UND[1]; 
	break;
case ARM_MODE_UND:
	break;
}
}

#pragma endregion

#pragma region Get And Set Registers

void __fastcall ARMCPU::load(unsigned int rn, unsigned int val)
{
    RON_USER[rn]=val;
}

void  __fastcall ARMCPU::loadusr(unsigned int n, unsigned int val)
{
if(n==15)
	{
	RON_USER[15]=val;
	return;
	}

switch(arm_mode_table[(CPSR&0x1f)|0x10])
	{
	case ARM_MODE_USER:
		RON_USER[n]=val;
		break;
	case ARM_MODE_FIQ:
		if(n>7) RON_CASH[n-8]=val;
		else RON_USER[n]=val;
		break;
	case ARM_MODE_IRQ:
	case ARM_MODE_ABT:
	case ARM_MODE_UND:
	case ARM_MODE_SVC:
		if(n>12)RON_CASH[n-8]=val;
		else RON_USER[n]=val;
		break;
	}
}

unsigned int __fastcall ARMCPU::rread(unsigned int rn)
{
return RON_USER[rn];
}


unsigned int __fastcall ARMCPU::rreadusr(unsigned int n)
{
if(n==15)return RON_USER[15];
switch(arm_mode_table[(CPSR&0x1f)])
	{
	case ARM_MODE_USER:
		return RON_USER[n];
	case ARM_MODE_FIQ:
		if(n>7)return RON_CASH[n-8];
		else return RON_USER[n];
	case ARM_MODE_IRQ:
	case ARM_MODE_ABT:
	case ARM_MODE_UND:
	case ARM_MODE_SVC:
		if(n>12)return RON_CASH[n-8];
		else return RON_USER[n];
	}
return 0;
}

#pragma endregion

void __inline __fastcall ARMCPU::mwritew(unsigned int addr, unsigned int val)
{
DMA->SetWord( addr, val );
}

unsigned int __inline __fastcall ARMCPU::mreadw(unsigned int addr)
{
return (int) DMA->GetWord( addr );
}

void __inline __fastcall ARMCPU::mwriteb(unsigned int addr, unsigned int val)
{
DMA->SetByte( addr, ( unsigned char ) val )  ;
}

unsigned int __inline __fastcall ARMCPU::mreadb(unsigned int addr)
{
return (int) DMA->GetByte ( addr );
}

int __fastcall ARMCPU::Execute(unsigned int MCLKs)
{
    uint32 cmd;    
    
	for(CYCLES=MCLKs; CYCLES>0; CYCLES-=SCYCLE)
	{
		
		cmd=mreadw(REG_PC&(~3));
			
		REG_PC+=4;

		if(MAS_Access_Exept)
		{
			//sprintf(str,"*PC: 0x%8.8X PrefAbort!!!\n",REG_PC);
			//CDebug::DPrint(str);
			//!!Exeption!!

			SPSR[arm_mode_table[ARM_MODE_CODE_ABT]]=CPSR;    
			SETI(1);
			SETM(ARM_MODE_CODE_ABT);
			load(14,REG_PC);    
			REG_PC=0x0000000C;  
			CYCLES-=SCYCLE+NCYCLE;
			MAS_Access_Exept=false;
			continue;
		}		
			

		if(((cond_flags_cross[(((uint32)cmd)>>28)]>>((CPSR)>>28))&1))
		{		

			switch((cmd>>24)&0xf)  //разбор типа команды
			{
			case 0x0:	//Multiply
					
					if ((cmd & ARM_MUL_MASK) == ARM_MUL_SIGN)
					{
						unsigned int res;

													
						res=((calcbits(RON_USER[(cmd>>8)&0xf])+5)>>1)-1;
						if(res>16)CYCLES-=16;
						else CYCLES-=res;

						if(((cmd>>16)&0xf)==(cmd&0xf))
						{
							if (cmd&(1<<21)) res=RON_USER[(cmd>>12)&0xf]+((((cmd>>12)&0xf)==0xf)?8:0);
							else res=0;	
						}
						else
						{
							if (cmd&(1<<21)) res=RON_USER[cmd&0xf]*RON_USER[(cmd>>8)&0xf]+RON_USER[(cmd>>12)&0xf]+((((cmd>>12)&0xf)==0xf)?8:0);
							else res=RON_USER[cmd&0xf]*RON_USER[(cmd>>8)&0xf];
						}							
						if(cmd&(1<<20))
						{
							ARM_SET_Z(res);
							ARM_SET_N(res);
						}
						RON_USER[(cmd>>16)&0xf]=res;
						
						break;
					}
			case 0x1:	//Single Data Swap
					if ((cmd & ARM_SDS_MASK) == ARM_SDS_SIGN)
					{
						SWAP(cmd);
						if(MAS_Access_Exept)
						{
							//sprintf(str,"*PC: 0x%8.8X DataAbort!!!\n",REG_PC);
							//CDebug::DPrint(str);
							//!!Exeption!!

							SPSR[arm_mode_table[ARM_MODE_CODE_ABT]]=CPSR;    
							SETI(1);
							SETM(ARM_MODE_CODE_ABT);
							load(14,REG_PC+4);    
							REG_PC=0x00000010;  
							CYCLES-=SCYCLE+NCYCLE;
							MAS_Access_Exept=false;
							break;
						}
						CYCLES-=2*NCYCLE+ICYCLE;						
						break;
					}
			case 0x2:	//ALU
			case 0x3:				
				
					
					uint32 op2,op1;
					uint8 shift,shtype;	
						
							  /////////////////////////////////////////////SHIFT
								if (cmd&(1<<25))
								{
									op2=cmd&0xff;
									if(((cmd>>7)&0x1e))
									{
										op2=ROTR(op2, (cmd>>7)&0x1e);	
										if((cmd&(1<<20))) SETC(((cmd&0xff)>>(((cmd>>7)&0x1e)-1))&1);									
									}
									op1=RON_USER[(cmd>>16)&0xf]+((((cmd>>16)&0xf)==0xf)?4:0);
								}
								else
								{
									shtype=(cmd>>5)&0x3;
									if(cmd&(1<<4))
									{
										shift=((cmd>>8)&0xf);
										shift=(RON_USER[shift]+((shift==0xf)?4:0))&0xff;
										op2=RON_USER[cmd&0xf]+(((cmd&0xf)==0xf)?8:0);
										op1=RON_USER[(cmd>>16)&0xf]+((((cmd>>16)&0xf)==0xf)?8:0);
										CYCLES-=ICYCLE;
									}
									else
									{
										shift=(cmd>>7)&0x1f;
										if(shift==0)
										{
											if(shtype!=0)
											{
												if(shtype==3)shtype++;
												else
												{
													shift=32;
												}
											}
										}
										op2=RON_USER[cmd&0xf]+(((cmd&0xf)==0xf)?4:0);
										op1=RON_USER[(cmd>>16)&0xf]+((((cmd>>16)&0xf)==0xf)?4:0);
									}

									if(cmd&(1<<20)) op2=SHIFT_SC(op2, shift, shtype);
									else op2=SHIFT_NSC(op2, shift, shtype);
										
								}

							  
							  if(ALU_Exec(cmd, (cmd>>20)&0x1f ,op1,op2,&RON_USER[(cmd>>12)&0xf]))
							  {									
										break;
							  }

							
							  if(((cmd>>12)&0xf)==0xf) //destination = pc, take care of cpsr
							  {
								if(cmd&(1<<20))
								{								
									SetCPSR(SPSR[arm_mode_table[MODE]]);
								}
								
								CYCLES-=ICYCLE+NCYCLE;
								
							  }
					break;	
			case 0x6:	//Undefined
			case 0x7:
				if((cmd&ARM_UND_MASK)==ARM_UND_SIGN)
				{  
					//sprintf(str,"*PC: 0x%8.8X undefined\n",REG_PC);
					//CDebug::DPrint(str);
					//!!Exeption!!

					SPSR[arm_mode_table[ARM_MODE_CODE_UND]]=CPSR;    
					SETI(1);
					SETM(ARM_MODE_CODE_UND);
					load(14,REG_PC);    
					REG_PC=0x00000004;  // (-4) fetch!!!
					CYCLES-=SCYCLE+NCYCLE; // +2S+1N
					break;
				}
			case 0x4:	//Single Data Transfer
			case 0x5:	  
			
				
					  unsigned int base;
					  unsigned int oper2;	
					  unsigned int tbas, val, rora;
					  uint8	delta;			  
					  
					  if(cmd&(1<<25))
					  {
					  				shtype=(cmd>>5)&0x3;
									if(cmd&(1<<4))
									{
										shift=((cmd>>8)&0xf);
										shift=(RON_USER[shift]+((shift==0xf)?4:0))&0xff;
										oper2=RON_USER[cmd&0xf]+(((cmd&0xf)==0xf)?8:0);
										delta=8;
									}
									else
									{
										shift=(cmd>>7)&0x1f;
										if(shift==0)
										{
											if(shtype!=0)
											{
												if(shtype==3)shtype++;
												else shift=32;
											}
										}
										oper2=RON_USER[cmd&0xf]+(((cmd&0xf)==0xf)?4:0);
										delta=4;
									}
							oper2=SHIFT_NSC(oper2, shift, shtype);
						  
					  }
					  else
					  {					
							oper2=(cmd&0xfff);											
							delta=4;
					  }

					  					  
					  if(((cmd>>16)&0xf)==0xf) base=RON_USER[((cmd>>16)&0xf)]+delta;
					  else base=RON_USER[((cmd>>16)&0xf)];
					 
					  tbas=base;		

					  if(!(cmd&(1<<23))) oper2=0-oper2;

					  if(cmd&(1<<24)) tbas=base=base+oper2;						  
					  else base=base+oper2;
						
					  if(cmd&(1<<20)) //load
					  {
						  if(cmd&(1<<22))//bytes
						  {
							val=mreadb(tbas)&0xff;
						  }
						  else //words/halfwords
						  {
							val=mreadw(tbas&0xfffffffc);    
							if((rora=tbas&3)) val=ROTR(val,rora*8);
						  }

						  if(((cmd>>12)&0xf)==0xf)
						  {
							  CYCLES-=SCYCLE+NCYCLE;   // +1S+1N if R15 load
							  val=(val)&0xfffffffc;
						  }

						  CYCLES-=NCYCLE+ICYCLE;  // +1N+1I

						  if ((cmd&(1<<21)) || (!(cmd&(1<<24)))) load((cmd>>16)&0xf,base);

						  if((cmd&(1<<21)) && !(cmd&(1<<24)))
							  loadusr((cmd>>12)&0xf,val);//privil mode
						  else
							  load((cmd>>12)&0xf,val);
						  
					  }
					  else
					  { // store
						  
						  if((cmd&(1<<21)) && !(cmd&(1<<24)))
						  	  val=rreadusr((cmd>>12)&0xf);// privil mode
						  else
							  val=rread((cmd>>12)&0xf); 
						  
						  if(((cmd>>12)&0xf)==0xf)val+=delta;

						  CYCLES-=-SCYCLE+2*NCYCLE;  // 2N

						  if(cmd&(1<<22))//bytes/words
						  	mwriteb(tbas,val);
						  else //words/halfwords
						  	mwritew(tbas,val);  
						  
						  if ( (cmd&(1<<21)) || !(cmd&(1<<24)) ) load((cmd>>16)&0xf,base);

					  }
					  

					  if(MAS_Access_Exept)
					  {
							//sprintf(str,"*PC: 0x%8.8X DataAbort!!!\n",REG_PC);
							//CDebug::DPrint(str);
						    //!!Exeption!!

							SPSR[arm_mode_table[ARM_MODE_CODE_ABT]]=CPSR;    
							SETI(1);
							SETM(ARM_MODE_CODE_ABT);
							load(14,REG_PC+4);    
							REG_PC=0x00000010;  
							CYCLES-=SCYCLE+NCYCLE;
							MAS_Access_Exept=false;
							break;
					  }//*/

					break;

			case 0x8:	//Block Data Transfer 
			case 0x9:

				{
				  bdt_core(cmd);
				  if(MAS_Access_Exept)
				  {
							//sprintf(str,"*PC: 0x%8.8X DataAbort!!!\n",REG_PC);
							//CDebug::DPrint(str);
							//!!Exeption!!

							SPSR[arm_mode_table[ARM_MODE_CODE_ABT]]=CPSR;    
							SETI(1);
							SETM(ARM_MODE_CODE_ABT);
							load(14,REG_PC+4);    
							REG_PC=0x00000010;  
							CYCLES-=SCYCLE+NCYCLE;
							MAS_Access_Exept=false;
							break;
				  }
				  
				}
				break;

			case 0xa:	//BRANCH
			case 0xb:		  			    					
					if(cmd&(1<<24))
					{					
						RON_USER[14]=REG_PC;
					}
					REG_PC+=(((cmd&0xffffff)|((cmd&0x800000)?0xff000000:0))<<2)+4;
				  
					CYCLES-=SCYCLE+NCYCLE; //2S+1N
				
					break;

			case 0xf:	//SWI		  				
					// TODO: IMPORT SWI CODE
					decode_swi(cmd);			  
					CYCLES-=0;
					
					break;
			//---------  
			default:	//сопроцессор
					//sprintf(str,"*PC: 0x%8.8X undefined\n",REG_PC);
					//CDebug::DPrint(str);
					//!!Exeption!!

					SPSR[arm_mode_table[ARM_MODE_CODE_UND]]=CPSR;    
					SETI(1);
					SETM(ARM_MODE_CODE_UND);
					load(14,REG_PC);    
					REG_PC=0x00000004;  
					CYCLES-=SCYCLE+NCYCLE; 
					break;

			};
		}	//condition
		
		

    	if(gFIQ && !ISF) 
			{
					// TODO: Figure out what madam is...
					//_madam_FSM=FSM_SUSPENDED;
					gFIQ=0;
					
					SPSR[arm_mode_table[ARM_MODE_CODE_FIQ]]=CPSR;
					SETF(1);
					SETI(1);
					SETM(ARM_MODE_CODE_FIQ);
					load(14,REG_PC+4);
					REG_PC=0x0000001c;//1c
			}

	} // for(CYCLES)
	return CYCLES;
}

//JMK NOTE:
//I've altered this code to use the "standard" DMA controller,
//because Altmer's code originally assumes that all the bytes
//it is reading are pre-swapped. 
//
//I'm unsure if this was really necessary to do, and replacing 
//this bdt_core with the original would certainly perform better!
void __inline ARMCPU::bdt_core(unsigned int opc)
{
 unsigned int base,i,rn_ind,bold;
 unsigned int addr;
 unsigned short list=opc&0xffff;
 
	//decode_mrt(opc);
	//return;

	rn_ind=(opc>>16)&0xf;

	if(rn_ind==0xf)base=RON_USER[rn_ind]+8;
	else base=RON_USER[rn_ind];

		
	
	if(opc&(1<<20))	//из памяти в регистр?
	{	
		if(opc&0x8000)CYCLES-=SCYCLE+NCYCLE;

		//addr=can_multy_read_direct(base, opc);	//проверка на безопасность
		addr=base;
		if(addr)	//возможна безопасная запись?
		{
			bold=base;
			if((opc&(1<<22)) && !(opc&0x8000)) //пользовательский банк регистров?
			{
				
				if(opc&(1<<23)) //инкремент?
				{
					i=0;
					if(opc&(1<<24))	//до?
					{
						while(list)
						{
							if(list&1){addr+=4;loadusr(i,mreadw(addr));base+=4;}
							i++;
							list>>=1;
						}
					}
					else
					{
						while(list)
						{
							if(list&1){loadusr(i,mreadw(addr));addr+=4;base+=4;}
							i++;
							list>>=1;
						}
					}
					
				}
				else	//декремент
				{
					i=14;
					list<<=1;
					if(opc&(1<<24))	//до?
					{
						while(list)
						{
							if(list&0x8000){addr-=4;loadusr(i,mreadw(addr));base-=4;}
							i--;
							list<<=1;
						}
					}
					else	//после
					{
						while(list)
						{
							if(list&0x8000){loadusr(i,mreadw(addr));addr-=4;base-=4;}
							i--;
							list<<=1;
						}
					}					
				}
				if((opc&(1<<21)) && !(opc&(1<<(rn_ind)))) RON_USER[rn_ind]=base;
				
			}
			else	//текущий банк регистров
			{
				
				if(opc&(1<<23))	//инкремент?
				{
					i=0;
					if(opc&(1<<24))
					{
						while(list)
						{
							if(list&1){addr+=4;RON_USER[i]=mreadw(addr);base+=4;}
							i++;
							list>>=1;
						}
					}
					else
					{
						while(list)
						{
							if(list&1){RON_USER[i]=mreadw(addr);addr+=4;base+=4;}
							i++;
							list>>=1;
						}
					}
				}
				else	//декримент
				{
					i=15;
					if(opc&(1<<24))
					{
						while(list)
						{
							if(list&0x8000){addr-=4;RON_USER[i]=mreadw(addr);base-=4;}
							i--;
							list<<=1;
						}
					}
					else
					{
						while(list)
						{
							if(list&0x8000){RON_USER[i]=mreadw(addr);addr-=4;base-=4;}
							i--;
							list<<=1;
						}
					}	
				}
				if((opc&(1<<21)) && !(opc&(1<<(rn_ind)))) RON_USER[rn_ind]=base;

				if((opc&(1<<22)) && arm_mode_table[MODE]) SetCPSR(SPSR[arm_mode_table[MODE]]);
				
			}
			
			CYCLES-=NCYCLE+ICYCLE-SCYCLE;
			if(base<bold)CYCLES-=SCYCLE*((bold-base)>>2);
			else CYCLES-=SCYCLE*((base-bold)>>2);
			 
		}
		else	//безопасная запись невозможна
		{
			ldm_accur(opc,base,rn_ind);
		}
	}
	else //из регистра в память
	{		
		if((opc&(1<<(rn_ind))))
		{
			stm_accur(opc,base,rn_ind);
			return;
		}
		//addr=can_multy_write_direct(base, opc);	//проверка на безопасность
		addr=base;
		if(addr)	//возможна безопасная запись?
		{
			bold=base;
			if(opc&(1<<22)) //работа с пользовательскими регистрами?
			{					
				if(opc&(1<<23)) //инкремент?
				{
					list&=0x7fff;
					i=0;					
					if(opc&(1<<24))	//до?
					{
						while(list)
						{
							if(list&1)
							{
								addr+=4;
								mwritew(addr,rreadusr(i));
								base+=4;
							}
							i++;
							list>>=1;
						}
						if(opc&0x8000)
						{
							addr+=4;
							base+=4;
							mwritew(addr,REG_PC+8);
						}
					}
					else
					{
						while(list)
						{
							if(list&1){mwritew(addr,rreadusr(i));addr+=4;base+=4;}
							i++;
							list>>=1;
						}
						if(opc&0x8000)
						{							
							mwritew(addr,REG_PC+8);
							addr+=4;
							base+=4;
						}
					}					
				}
				else	//декремент
				{
					i=14;
					list<<=1;
					if(opc&(1<<24))	//до?
					{
						if(opc&0x8000)
						{
							addr-=4;
							base-=4;
							mwritew(addr,REG_PC+8);
						}
						while(list)
						{
							if(list&0x8000){addr-=4;mwritew(addr,rreadusr(i));base-=4;}
							i--;
							list<<=1;
						}
					}
					else	//после
					{
						if(opc&0x8000)
						{							
							mwritew(addr,REG_PC+8);
							addr-=4;
							base-=4;
						}
						while(list)
						{
							if(list&0x8000){mwritew(addr,rreadusr(i));addr-=4;base-=4;}
							i--;
							list<<=1;
						}
					}					
				}
				if((opc&(1<<21))) RON_USER[rn_ind]=base;
			}
			else //работа с текущим банком
			{
				if(opc&(1<<23)) //инкремент?
				{
					list&=0x7fff;
					i=0;					
					if(opc&(1<<24))	//до?
					{
						while(list)
						{
							if(list&1)
							{
								addr+=4;								
								mwritew(addr,RON_USER[i]);
								base+=4;
							}
							i++;
							list>>=1;
						}
						if(opc&0x8000)
						{
							addr+=4;
							base+=4;
							mwritew(addr,REG_PC+8);
						}
					}
					else
					{
						while(list)
						{
							if(list&1){mwritew(addr,RON_USER[i]);addr+=4;base+=4;}
							i++;
							list>>=1;
						}
						if(opc&0x8000)
						{							
							mwritew(addr,REG_PC+8);
							addr+=4;
							base+=4;
						}
					}					
				}
				else	//декремент
				{
					i=14;
					list<<=1;
					if(opc&(1<<24))	//до?
					{
						if(opc&0x8000)
						{
							addr-=4;
							base-=4;
							mwritew(addr,REG_PC+8);
						}
						while(list)
						{
							if(list&0x8000){addr-=4;mwritew(addr,RON_USER[i]);base-=4;}
							i--;
							list<<=1;							
						}
					}
					else	//после
					{
						if(opc&0x8000)
						{							
							mwritew(addr,REG_PC+8);
							addr-=4;
							base-=4;
						}
						while(list)
						{
							if(list&0x8000){mwritew(addr,RON_USER[i]);addr-=4;base-=4;}
							i--;
							list<<=1;							
						}
					}					
				}
				if((opc&(1<<21))) RON_USER[rn_ind]=base;				
			}

			CYCLES-=NCYCLE+NCYCLE-SCYCLE-SCYCLE;
			if(base<bold)CYCLES-=SCYCLE*((bold-base)>>2);
			else CYCLES-=SCYCLE*((base-bold)>>2);
		}
		else
		{
			stm_accur(opc,base,rn_ind);
		}
	}
	
}

__inline void ARMCPU::ldm_accur(unsigned int opc, unsigned int base, unsigned int rn_ind)
{
 unsigned short x=opc&0xffff;
 unsigned short list=opc&0xffff;
 unsigned int base_comp,	//по ней шагаем
				i=0,tmp;
 
	x = (x & 0x5555) + ((x >> 1) & 0x5555); 
	x = (x & 0x3333) + ((x >> 2) & 0x3333); 
	x = (x & 0xff) + (x >> 8);
	x = (x & 0xf) + (x >> 4);
	
	switch((opc>>23)&3)
	{
	 case 0:
		base-=(x<<2);
		base_comp=base+4;
		break;
	 case 1:
		base_comp=base;
		base+=(x<<2);
		break;
	 case 2:
		base_comp=base=base-(x<<2);
		break;
	 case 3:
		base_comp=base+4;
		base+=(x<<2);
		break;
	}

	//base_comp&=~3;

	if(opc&(1<<21))RON_USER[rn_ind]=base;
	
	if((opc&(1<<22)) && !(opc&0x8000))
	{
		while(list)
		{
			if(list&1)
			{
				tmp=mreadw(base_comp);
				if(MAS_Access_Exept)
				{
					if(opc&(1<<21))RON_USER[rn_ind]=base;
					break;
				}
				loadusr(i,tmp);				
				base_comp+=4;
			}
			i++;
			list>>=1;
		}
	}
	else
	{
		while(list)
		{
			if(list&1)
			{
				tmp=mreadw(base_comp);
				if(MAS_Access_Exept)
				{
					if(opc&(1<<21))RON_USER[rn_ind]=base;
					break;
				}
				RON_USER[i]=tmp;	
				base_comp+=4;
			}
			i++;
			list>>=1;
		}
		if((opc&(1<<22)) && arm_mode_table[MODE] && !MAS_Access_Exept) SetCPSR(SPSR[arm_mode_table[MODE]]);
	}

	CYCLES-=(x-1)*SCYCLE+NCYCLE+ICYCLE;

}


__inline void ARMCPU::stm_accur(unsigned int opc, unsigned int base, unsigned int rn_ind)
{
 unsigned short x=opc&0xffff;
 unsigned short list=opc&0x7fff;
 unsigned int base_comp,	//по ней шагаем
				i=0;
 
	x = (x & 0x5555) + ((x >> 1) & 0x5555); 
	x = (x & 0x3333) + ((x >> 2) & 0x3333); 
	x = (x & 0xff) + (x >> 8);
	x = (x & 0xf) + (x >> 4);
	
	switch((opc>>23)&3)
	{
	 case 0:
		base-=(x<<2);
		base_comp=base+4;
		break;
	 case 1:
		base_comp=base;
		base+=(x<<2);
		break;
	 case 2:
		base_comp=base=base-(x<<2);
		break;
	 case 3:
		base_comp=base+4;
		base+=(x<<2);
		break;
	}

	//base_comp&=~3;

	if(opc&(1<<21) && rn_ind)RON_USER[rn_ind]=base;
	
	if((opc&(1<<22)))
	{
		while(list)
		{
			if(list&1)
			{
				mwritew(base_comp,rreadusr(i));
				if(MAS_Access_Exept)break;							
				base_comp+=4;
			}
			i++;
			list>>=1;
		}
	}
	else
	{
		while(list)
		{
			if(list&1)
			{
				mwritew(base_comp,RON_USER[i]);
				if(MAS_Access_Exept)break;							
				base_comp+=4;
			}
			i++;
			list>>=1;
		}
		
	}

	if((opc&0x8000) && !MAS_Access_Exept)mwritew(base_comp,RON_USER[15]+8);
	if(opc&(1<<21))RON_USER[rn_ind]=base;

	CYCLES-=(x-2)*SCYCLE+NCYCLE+NCYCLE;

}

__inline unsigned int* ARMCPU::can_multy_read_direct(unsigned int base, unsigned int )
{
	return (unsigned int *)DMA->GetRAMPointer( base );
/*
 unsigned int index;
	base&=~3;
	if(opc&(1<<23))
	{
		if (base<0x00300000 && (base+68)<0x00300000) //dram1&dram2&vram
		{
			return (unsigned int*)(pRam+base);
			
		}
		else if (!((index=(base^0x03000000)) & ~0xFFFFF) && !(((base+68)^0x03000000) & ~0xFFFFF)) //rom
		{
			if(!gSecondROM) // 2nd rom
			    return (unsigned int*)(pRom+index);
			else
			    return (unsigned int*)(pRom+index+1024*1024);
		}
	}
	else
	{
		if (base<0x00300000 && ((unsigned int)(base-68))<0x00300000) //dram1&dram2&vram
		{
			return (unsigned int*)(pRam+base);
			
		}
		else if (!((index=(base^0x03000000)) & ~0xFFFFF) && !(((base-68)^0x03000000) & ~0xFFFFF)) //rom
		{
			if(!gSecondROM) // 2nd rom
			    return (unsigned int*)(pRom+index);
			else
			    return (unsigned int*)(pRom+index+1024*1024);
		}
	}
	return NULL;
*/
}


__inline unsigned int* ARMCPU::can_multy_write_direct(unsigned int base, unsigned int )
{
	return (unsigned int *)DMA->GetRAMPointer( base );
/*
	base&=~3;
	if(opc&(1<<23))
	{
		if (base<0x00300000 && (base+68)<0x00300000) //dram1&dram2&vram
		{
			return (unsigned int*)(pRam+base);			
		}
	}
	else
	{
		if (base<0x00300000 && ((unsigned int)(base-68))<0x00300000) //dram1&dram2&vram
		{
			return (unsigned int*)(pRam+base);			
		}
	}
	return NULL;
*/
}

void inline __fastcall ARMCPU::decode_swi(unsigned int cmd)
{
    CYCLES-=SCYCLE+NCYCLE; // +2S+1N

	uint32  swi;
	
	swi = cmd & 0x00FFFFFF;

	SPSR[arm_mode_table[ARM_MODE_CODE_SVC]]=CPSR;  // Save CPSR in SPSR_svc  
	SETI(1);						               // Set interrupt status flag
	SETM(ARM_MODE_CODE_SVC);				       // Go into supervisor mode
	load(14,REG_PC+4);							   // Save address of instruction (+4) into R14_svc
	REG_PC=0x00000008;			                   // Force PC to fetch next instruction from vector table
	CYCLES-=SCYCLE+NCYCLE;
	
    // JMK NOTE: This is the goal line! Woo!
    
    /*
    SPSR[arm_mode_table[ARM_MODE_CODE_SVC]]=CPSR;
    
    SETI(1);
    SETM(ARM_MODE_CODE_SVC);

	load(14,REG_PC);
    
    REG_PC=0x00000008;  
	*/
}