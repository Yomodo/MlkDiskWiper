#pragma once

#include <winioctl.h>

#define EXTERN_DLL_EXPORT extern "C" __declspec(dllexport)

EXTERN_DLL_EXPORT DWORD HelloWorld();
