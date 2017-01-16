namespace MlkDiskWiper.PlatformInvoke.ntddisk
{
    public enum MbrPartitionType : byte {
        PARTITION_ENTRY_UNUSED          = 0x00,     // Entry unused
        PARTITION_FAT_12                = 0x01,     // 12-bit FAT entries
        PARTITION_XENIX_1               = 0x02,     // Xenix
        PARTITION_XENIX_2               = 0x03,     // Xenix
        PARTITION_FAT_16                = 0x04,     // 16-bit FAT entries
        PARTITION_EXTENDED              = 0x05,     // Extended partition entry
        PARTITION_HUGE                  = 0x06,     // Huge partition MS-DOS V4
        PARTITION_IFS                   = 0x07,     // IFS Partition
        PARTITION_OS2BOOTMGR            = 0x0A,     // OS/2 Boot Manager/OPUS/Coherent swap
        PARTITION_FAT32                 = 0x0B,     // FAT32
        PARTITION_FAT32_XINT13          = 0x0C,     // FAT32 using extended int13 services
        PARTITION_XINT13                = 0x0E,     // Win95 partition using extended int13 services
        PARTITION_XINT13_EXTENDED       = 0x0F,     // Same as type 5 but uses extended int13 services
        PARTITION_PREP                  = 0x41,     // PowerPC Reference Platform (PReP) Boot Partition
        PARTITION_LDM                   = 0x42,     // Logical Disk Manager partition
        PARTITION_DM                    = 0x54,     // OnTrack Disk Manager partition
        PARTITION_EZDRIVE               = 0x55,     // EZ-Drive partition
        PARTITION_UNIX                  = 0x63,     // Unix
        PARTITION_SPACES                = 0xE7,     // Storage Spaces protective partition
        PARTITION_GPT                   = 0xEE,     // Gpt protective partition

        VALID_NTFT                      = 0xC0,     // NTFT uses high order bits

        //
        // The high bit of the partition=  type code indicates that a partition
        // is part of an NTFT mirror or = striped array.
        //
        PARTITION_NTFT                  = 0x80,     // NTFT partition
    }
}
