#include "stdafx.h"

#include "IoCtl.h"

EXTERN_DLL_EXPORT BOOL WINAPI IoctlDiskGetDriveGeometryEx(
	HANDLE device,
	IoctlDiskGetDriveGeometryEx_Result *result)
{
	BOOL success = false;

	ZeroMemory(result, sizeof(result));

	size_t diskGeometrySize = offsetof(DISK_GEOMETRY_EX, Data) + sizeof(DISK_PARTITION_INFO) + sizeof(DISK_DETECTION_INFO);
	PDISK_GEOMETRY_EX diskGeometry = (PDISK_GEOMETRY_EX)malloc(diskGeometrySize);
	if (!diskGeometry)
		goto cleanup;
	ZeroMemory(diskGeometry, diskGeometrySize);

	DWORD bytesReturned;
	if (!DeviceIoControl(
		device,								// handle to volume
		IOCTL_DISK_GET_DRIVE_GEOMETRY_EX,	// dwIoControlCode
		nullptr,                            // lpInBuffer
		0,									// nInBufferSize
		diskGeometry,						// output buffer
		diskGeometrySize,					// size of output buffer
		&bytesReturned,						// number of bytes returned
		nullptr))							// OVERLAPPED structure
	{
		goto cleanup;
	}

	PDISK_PARTITION_INFO partitionInfo = DiskGeometryGetPartition(diskGeometry);
	PDISK_DETECTION_INFO detectionInfo = DiskGeometryGetDetect(diskGeometry);

	result->DiskSize = diskGeometry->DiskSize;
	result->Cylinders = diskGeometry->Geometry.Cylinders;
	result->TracksPerCylinder = diskGeometry->Geometry.TracksPerCylinder;
	result->SectorsPerTrack = diskGeometry->Geometry.SectorsPerTrack;
	result->BytesPerSector = diskGeometry->Geometry.BytesPerSector;

	switch (partitionInfo->PartitionStyle)
	{
	case PARTITION_STYLE_GPT:
		result->DiskId = partitionInfo->Gpt.DiskId;
		break;
	case PARTITION_STYLE_MBR:
		result->DiskSignature = partitionInfo->Mbr.Signature;
		break;
	}

	success = TRUE;

cleanup:
	free(diskGeometry);
	return success;
}

EXTERN_DLL_EXPORT BOOL WINAPI IoctlStorageGetDeviceNumber(
	HANDLE device,
	PSTORAGE_DEVICE_NUMBER deviceNumber)
{
	DWORD bytesReturned;

	return DeviceIoControl(
		device,
		IOCTL_STORAGE_GET_DEVICE_NUMBER,
		nullptr,
		0,
		deviceNumber,
		sizeof(STORAGE_DEVICE_NUMBER),
		&bytesReturned,
		nullptr);
}

EXTERN_DLL_EXPORT BOOL WINAPI IoctlDiskGetLengthInfo(
	HANDLE device,
	PLARGE_INTEGER length)
{
	DWORD bytesReturned;
	GET_LENGTH_INFORMATION getLengthInformation = {};

	if (!DeviceIoControl(
		device,
		IOCTL_DISK_GET_LENGTH_INFO,
		nullptr,
		0,
		&getLengthInformation,
		sizeof(getLengthInformation),
		&bytesReturned,
		nullptr))
	{
		return FALSE;
	}

	*length = getLengthInformation.Length;

	return TRUE;
}

EXTERN_DLL_EXPORT bool WINAPI IoctlCreateMbrDisk(
	HANDLE device,
	DWORD uniqueSignature)
{
	CREATE_DISK diskInfo = {};
	diskInfo.PartitionStyle = PARTITION_STYLE_MBR;
	diskInfo.Mbr.Signature = uniqueSignature;

	DWORD bytesReturned;

	return DeviceIoControl(device,	// handle to device
		IOCTL_DISK_CREATE_DISK,		// dwIoControlCode
		&diskInfo,					// input buffer
		sizeof(diskInfo),			// size of input buffer
		nullptr,					// lpOutBuffer
		0,							// nOutBufferSize
		&bytesReturned,				// number of bytes returned
		nullptr);					// OVERLAPPED structure
}

EXTERN_DLL_EXPORT bool WINAPI IoctlSetMbrDriveLayout(
	HANDLE device,
	DWORD uniqueSignature,
	LARGE_INTEGER startingOffset,
	LARGE_INTEGER partitionLength,
	BYTE partitionType,
	BOOL bootable,
	DWORD hiddenSectors)
{
	DWORD result;

	size_t layoutInfoSize = offsetof(DRIVE_LAYOUT_INFORMATION_EX, PartitionEntry) + sizeof(PARTITION_INFORMATION_EX)*4;

	DRIVE_LAYOUT_INFORMATION_EX *layoutInfo = (DRIVE_LAYOUT_INFORMATION_EX*)malloc(layoutInfoSize);
	if (layoutInfo == nullptr)
		goto abort;
	ZeroMemory(layoutInfo, layoutInfoSize);

	layoutInfo->PartitionStyle = PARTITION_STYLE_MBR;
	layoutInfo->PartitionCount = 1;
	layoutInfo->Mbr.Signature = uniqueSignature;

	for (int i = 0; i < 4; i++)
	{
		layoutInfo->PartitionEntry[i].PartitionStyle = PARTITION_STYLE_MBR;
		layoutInfo->PartitionEntry[i].StartingOffset.QuadPart = 0;
		layoutInfo->PartitionEntry[i].PartitionLength.QuadPart = 0;
		layoutInfo->PartitionEntry[i].PartitionNumber = i + 1;
		layoutInfo->PartitionEntry[i].RewritePartition = TRUE;
		layoutInfo->PartitionEntry[i].Mbr.PartitionType = PARTITION_ENTRY_UNUSED;
		layoutInfo->PartitionEntry[i].Mbr.BootIndicator = FALSE;
		layoutInfo->PartitionEntry[i].Mbr.RecognizedPartition = FALSE;
		layoutInfo->PartitionEntry[i].Mbr.HiddenSectors = 0;
	}

	layoutInfo->PartitionEntry[0].StartingOffset = startingOffset;
	layoutInfo->PartitionEntry[0].PartitionLength = partitionLength;
	layoutInfo->PartitionEntry[0].Mbr.PartitionType = partitionType;
	layoutInfo->PartitionEntry[0].Mbr.BootIndicator = bootable;
	layoutInfo->PartitionEntry[0].Mbr.HiddenSectors = hiddenSectors;

	DWORD bytesReturned;

	if (!DeviceIoControl(
		device,							// handle to device
		IOCTL_DISK_SET_DRIVE_LAYOUT_EX,	// dwIoControlCode
		layoutInfo,						// input buffer
		layoutInfoSize,					// size of the input buffer
		nullptr,                        // lpOutBuffer
		0,                              // nOutBufferSize 
		&bytesReturned,					// number of bytes returned
		nullptr))						// OVERLAPPED structure
	{
		result = FALSE;
		goto abort;
	}

	result = TRUE;

abort:
	free(layoutInfo);
	return result;
}

EXTERN_DLL_EXPORT bool WINAPI IoctlCreateGptDisk(
	HANDLE  device,
	GUID diskId,
	DWORD maxParttitionCount)
{
	CREATE_DISK diskInfo = {};
	diskInfo.PartitionStyle = PARTITION_STYLE_GPT;
	diskInfo.Gpt.DiskId = diskId;
	diskInfo.Gpt.MaxPartitionCount = maxParttitionCount;

	DWORD bytesReturned;

	return DeviceIoControl(device,	// handle to device
		IOCTL_DISK_CREATE_DISK,		// dwIoControlCode
		&diskInfo,					// input buffer
		sizeof(diskInfo),			// size of input buffer
		nullptr,					// lpOutBuffer
		0,							// nOutBufferSize
		&bytesReturned,				// number of bytes returned
		nullptr);					// OVERLAPPED structure
}

EXTERN_DLL_EXPORT bool WINAPI IoctlSetGptDriveLayout(
	HANDLE device,
	GUID diskId,
	DWORD uniqueSignature,
	LARGE_INTEGER startingOffset,
	LARGE_INTEGER partitionLength,
	BYTE partitionType,
	BOOL bootable,
	DWORD hiddenSectors)
{
	DRIVE_LAYOUT_INFORMATION_EX layoutInfo = {};

	layoutInfo.PartitionStyle = PARTITION_STYLE_GPT;
	layoutInfo.PartitionCount = 1;
	layoutInfo.Gpt.DiskId = diskId;
	layoutInfo.Gpt.StartingUsableOffset.QuadPart = 0;

	DWORD bytesReturned;

	return DeviceIoControl(
		device,							// handle to device
		IOCTL_DISK_SET_DRIVE_LAYOUT_EX,	// dwIoControlCode
		&layoutInfo,					// input buffer
		sizeof(layoutInfo),				// size of the input buffer
		nullptr,                        // lpOutBuffer
		0,                              // nOutBufferSize 
		&bytesReturned,					// number of bytes returned
		nullptr);						// OVERLAPPED structure

}

EXTERN_DLL_EXPORT bool WINAPI IoctlDiskUpdateProperties(HANDLE device)
{
	DWORD bytesReturned;

	return DeviceIoControl(
		device,							// handle to device
		IOCTL_DISK_UPDATE_PROPERTIES,	// dwIoControlCode
		nullptr,                        // lpInBuffer
		0,								// nInBufferSize
		nullptr,                        // lpOutBuffer
		0,								// nOutBufferSize
		&bytesReturned,					// lpBytesReturned
		nullptr);						// lpOverlapped
}

static struct _IoctlDiskGetDriveLayout_Enumerator
{
	DWORD ParitionStyle;
	long i;
	long Total;
	IoctlDiskGetDriveLayout_PartitionInfo Current;
	HANDLE handle;
};

EXTERN_DLL_EXPORT BOOL WINAPI IoctlDiskGetDriveLayout_Enumerate(
	HANDLE device,
	IoctlDiskGetDriveLayout_Enumerator *enumerator)
{
	DRIVE_LAYOUT_INFORMATION_EX layoutInfo = {};

	//if (!DeviceIoControl(
	//	device,							// handle to device
	//	IOCTL_DISK_GET_DRIVE_LAYOUT_EX,	// dwIoControlCode
	//	nullptr,                        // lpInBuffer
	//	0,								// nInBufferSize
	//	(LPVOID)lpOutBuffer,			// output buffer
	//	(DWORD)nOutBufferSize,			// size of output buffer
	//	(LPDWORD)lpBytesReturned,		// number of bytes returned
	//	(LPOVERLAPPED)lpOverlapped))	// OVERLAPPED structure
	//{
	//	return FALSE;
	//}
	return TRUE;
}

EXTERN_DLL_EXPORT void WINAPI IoctlDiskGetDriveLayout_Done(
	IoctlDiskGetDriveLayout_Enumerator *enumerator)
{
}

static struct _IoctlVolumeGetVolumeDiskExtents_Enumerator
{
	int i;
	DISK_EXTENT current;
	PVOLUME_DISK_EXTENTS extents;
};

EXTERN_DLL_EXPORT BOOL WINAPI IoctlVolumeGetVolumeDiskExtents_Enumerate(
	HANDLE device,
	IoctlVolumeGetVolumeDiskExtents_Enumerator *enumerator)
{
	_IoctlVolumeGetVolumeDiskExtents_Enumerator *e = (_IoctlVolumeGetVolumeDiskExtents_Enumerator*)enumerator;

	e->i = -1;
	ZeroMemory(&e->current, sizeof(DISK_EXTENT));
	e->extents = nullptr;

	DWORD pExtentsSize = sizeof(VOLUME_DISK_EXTENTS);
	e->extents = (PVOLUME_DISK_EXTENTS)malloc(pExtentsSize);
	if (e->extents == nullptr)
		return FALSE;
	ZeroMemory(e->extents, pExtentsSize);
	
	DWORD bytesReturned;

	if (!DeviceIoControl(
		device,
		IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS,
		nullptr,
		0,
		e->extents,
		pExtentsSize,
		&bytesReturned,
		nullptr))
	{
		free(e->extents);

		if (GetLastError() != ERROR_MORE_DATA)
			return FALSE;

		pExtentsSize = offsetof(VOLUME_DISK_EXTENTS, Extents) + e->extents->NumberOfDiskExtents * sizeof(DISK_EXTENT);
		e->extents = (PVOLUME_DISK_EXTENTS)malloc(pExtentsSize);
		if (e->extents == nullptr)
			return FALSE;
		ZeroMemory(e->extents, pExtentsSize);

		if (!DeviceIoControl(
			device,
			IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS,
			nullptr,
			0,
			e->extents,
			pExtentsSize,
			&bytesReturned,
			nullptr))
		{
			free(e->extents);
			return FALSE;
		}
	}

	return TRUE;
}

EXTERN_DLL_EXPORT BOOL WINAPI IoctlVolumeGetVolumeDiskExtents_Next(
	IoctlVolumeGetVolumeDiskExtents_Enumerator *enumerator)
{
	_IoctlVolumeGetVolumeDiskExtents_Enumerator *e = (_IoctlVolumeGetVolumeDiskExtents_Enumerator*)enumerator;

	e->i++;
	int i = e->i;

	long numberOfExtents = (long)e->extents->NumberOfDiskExtents;
	if (numberOfExtents <= i)
		return FALSE;

	e->current = e->extents->Extents[i];

	return TRUE;
}

EXTERN_DLL_EXPORT void WINAPI IoctlVolumeGetVolumeDiskExtents_Done(
	IoctlVolumeGetVolumeDiskExtents_Enumerator *enumerator)
{
	_IoctlVolumeGetVolumeDiskExtents_Enumerator *e = (_IoctlVolumeGetVolumeDiskExtents_Enumerator*)enumerator;

	free(e->extents);
}
