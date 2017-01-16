#include "stdafx.h"

#include <InitGuid.h>
#include <vds.h>

#pragma comment( lib, "ole32.lib" )
#pragma comment( lib, "rpcrt4.lib" )

#include "VirtualDiskService.h"

EXTERN_DLL_EXPORT BOOL WINAPI GetDisks()
{
	BOOL result = FALSE;

	IVdsServiceLoader *pLoader = nullptr;

	if (!CoCreateInstance(
		CLSID_VdsLoader,
		nullptr,
		CLSCTX_LOCAL_SERVER,
		IID_IVdsServiceLoader,
		(LPVOID*)&pLoader))
	{
		goto cleanup;
	}

	IVdsService* pService = nullptr;

	if (!pLoader->LoadService(nullptr, &pService))
		goto cleanup;

	if (!pService->WaitForServiceReady())
		goto cleanup;

	result = TRUE;

cleanup:
	if (pLoader != nullptr)
		pLoader->Release();

	return result;
}