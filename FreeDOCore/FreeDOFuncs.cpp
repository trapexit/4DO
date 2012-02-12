// FreeDOFuncs.cpp
// compile with: /EHsc /LD

#include "FreeDOFuncs.h"
#include "freedocore.h"

#include <stdexcept>

using namespace std;

// This is an extern FreeDO uses.
int __tex__scaler = 0;

/*
JMK NOTE: I started adding "easier" exports... these can maybe be destroyed.

FREEDOCORE_API int FreeDO_Init(_ext_Interface externalInterface)
{
	return 0;
}

FREEDOCORE_API void FreeDO_Destroy()
{
	return;
}

FREEDOCORE_API void FreeDO_Execute_Frame(VDLFrame* frame)
{
	return;
}

FREEDOCORE_API void FreeDO_Execute_Frame_Multitask(VDLFrame* frame)
{
	return;
}

FREEDOCORE_API void FreeDO_Do_Frame_Multitask(VDLFrame* frame)
{
	return;
}

FREEDOCORE_API unsigned int FreeDO_Get_Save_Size()
{
	return 0;
}

FREEDOCORE_API unsigned int FreeDO_Get_Save_Size()
{
	return 1;
}

FREEDOCORE_API void FreeDO_Do_Save(void* buffer)
{
	return;
}

FREEDOCORE_API bool FreeDO_Do_Load(void* buffer)
{
	return true;
}

FREEDOCORE_API void* FreeDO_Get_Pointer_NVRAM()
{
	return true;
}

         case FDP_DO_SAVE:
                _3do_Save(datum);
                break;
         case FDP_DO_LOAD:
                return (void*)_3do_Load(datum);
         case FDP_GETP_NVRAM:
                return Getp_NVRAM();
         case FDP_GETP_RAMS:
                return Getp_RAMS();
         case FDP_GETP_ROMS:
                return Getp_ROMS();
         case FDP_GETP_PROFILE:
                return profiling;
         case FDP_FREEDOCORE_VERSION:
                return (void*)0x20008;
         case FDP_SET_ARMCLOCK:
                ARM_CLOCK=(int)datum;
                break;
         case FDP_SET_TEXQUALITY:
                __tex__scaler=(int)datum;
                break; 

*/