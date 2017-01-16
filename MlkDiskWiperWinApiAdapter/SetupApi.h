#pragma once
#pragma comment(lib, "Setupapi.lib")

#include <SetupAPI.h>

#include "MlkDiskWiperWinApiAdapter.h"

static const int SetupApiDeviceEnumeratorPathSize = 2048;

struct SetupApiDeviceEnumerator {
	int i;
	BOOL wasError;
	WCHAR current[SetupApiDeviceEnumeratorPathSize];
	HANDLE handle;
};

EXTERN_DLL_EXPORT BOOL WINAPI SetupApi_Enumerate(
	GUID classGuid,
	DWORD flags,
	SetupApiDeviceEnumerator *enumerator);

EXTERN_DLL_EXPORT BOOL WINAPI SetupApi_Next(SetupApiDeviceEnumerator *enumerator);

EXTERN_DLL_EXPORT BOOL WINAPI SetupApi_ReadProperty(
	_In_      SetupApiDeviceEnumerator	*enumerator,
	_In_	  DWORD						property,
	_Out_opt_ PDWORD					propertyRegDataType,
	_Out_opt_ PBYTE						propertyBuffer,
	_In_      DWORD						propertyBufferSize,
	_Out_opt_ PDWORD					requiredSize);

EXTERN_DLL_EXPORT void WINAPI SetupApi_Done(SetupApiDeviceEnumerator *enumerator);
