#pragma once

#include "MlkDiskWiperWinApiAdapter.h"

typedef struct {
	LARGE_INTEGER DiskSize;
	LARGE_INTEGER Cylinders;
	DWORD TracksPerCylinder;
	DWORD SectorsPerTrack;
	DWORD BytesPerSector;
	DWORD DiskSignature;
	GUID DiskId;
} IoctlDiskGetDriveGeometryEx_Result;

EXTERN_DLL_EXPORT BOOL WINAPI IoctlDiskGetDriveGeometryEx(
	HANDLE device,
	IoctlDiskGetDriveGeometryEx_Result *result);

EXTERN_DLL_EXPORT BOOL WINAPI IoctlStorageGetDeviceNumber(
	HANDLE device,
	PSTORAGE_DEVICE_NUMBER deviceNumber);

EXTERN_DLL_EXPORT BOOL WINAPI IoctlDiskGetLengthInfo(
	HANDLE device,
	PLARGE_INTEGER length);

EXTERN_DLL_EXPORT bool WINAPI IoctlCreateGptDisk(
	HANDLE  device,
	GUID diskId,
	DWORD maxParttitionCount);

EXTERN_DLL_EXPORT bool WINAPI IoctlCreateMbrDisk(
	HANDLE device,
	DWORD uniqueSignature);

EXTERN_DLL_EXPORT bool WINAPI IoctlDiskUpdateProperties(HANDLE device);

struct IoctlDiskGetDriveLayout_PartitionInfo
{
	LARGE_INTEGER StartingOffset;
	LARGE_INTEGER PartitionLength;
	DWORD PartitionNumber;
	BYTE MbrParititonType;
	BOOLEAN MbrIsRecognizedType;
	BOOLEAN MbrIsBootable;
	DWORD MbrHiddenSectorCount;
	GUID PartitionType;
	GUID PartitionId;
	DWORD64 Attributes;
	WCHAR Name[36];
};

struct IoctlDiskGetDriveLayout_Enumerator
{
	DWORD ParitionStyle;
	long i;
	long Total;
	IoctlDiskGetDriveLayout_PartitionInfo Current;
	HANDLE handle;
};

struct IoctlVolumeGetVolumeDiskExtents_Enumerator
{
	int i;
	DISK_EXTENT current;
	HANDLE handle;
};

EXTERN_DLL_EXPORT BOOL WINAPI IoctlVolumeGetVolumeDiskExtents_Enumerate(
	HANDLE device,
	IoctlVolumeGetVolumeDiskExtents_Enumerator *enumerator);

EXTERN_DLL_EXPORT BOOL WINAPI IoctlVolumeGetVolumeDiskExtents_Next(
	IoctlVolumeGetVolumeDiskExtents_Enumerator *enumerator);

EXTERN_DLL_EXPORT void WINAPI IoctlVolumeGetVolumeDiskExtents_Done(
	IoctlVolumeGetVolumeDiskExtents_Enumerator *enumerator);
