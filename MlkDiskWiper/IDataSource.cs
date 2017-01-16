using System;

namespace MlkDiskWiper
{
    public interface IDataSource : IDisposable
    {
        void GetBytes(byte[] data);
    }
}
