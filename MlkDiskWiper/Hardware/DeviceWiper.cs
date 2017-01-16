using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace MlkDiskWiper.Hardware
{
    public static class DeviceWiper
    {
        public static void WipePhysicalDrive(Func<IDataSource> sourceFactory, int driveNumber, IProgress<int> progress, CancellationToken cancelToken)
        {
            Wipe(sourceFactory, $@"\\?\PhysicalDrive{driveNumber}", progress, cancelToken);
        }

        public static void Wipe(Func<IDataSource> sourceFactory, string destinationPath, IProgress<int> progress, CancellationToken cancelToken)
        {
            using (var fileHandle = FileOpener.OpenPath(destinationPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            using (var fileStream = new FileStream(fileHandle, FileAccess.ReadWrite))
            {
                Wipe(sourceFactory, fileStream, progress, cancelToken);
            }
        }

        public static void Wipe(Func<IDataSource> sourceFactory, FileStream destination, IProgress<int> progress, CancellationToken cancelToken)
        {
            progress.Report(0);
            destination.Seek(0, SeekOrigin.Begin);

            var diskSize = IoctlQueryer.GetDiskLength(destination.SafeFileHandle);
            var sectorSize = GetHighestPowerOf2(diskSize, max: 1024 * 1024);

            var buffer = new byte[sectorSize];

            using (var source = sourceFactory())
            {
                for (long i = 0; i < diskSize; i += sectorSize)
                {
                    cancelToken.ThrowIfCancellationRequested();

                    source.GetBytes(buffer);
                    destination.Write(buffer, 0, buffer.Length);
                    progress.Report(GetPercentage(i, diskSize));
                }
            }
        }

        static int GetHighestPowerOf2(long number, int max)
        {
            var powersOf2 = Enumerable.Range(0, sizeof(int) * 8)
                .Select(i => 1 << i)
                .Where(i => 0 < i && i <= max)
                .Reverse();
            return powersOf2.First(i => number % i == 0);
        }

        static int GetPercentage(long val, long total)
        {
            var percent = ((double)val) / total;
            return (int)Math.Round(percent * 100);
        }
    }
}
