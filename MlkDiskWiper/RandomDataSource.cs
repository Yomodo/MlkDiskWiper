using System;
using System.Security.Cryptography;

namespace MlkDiskWiper
{
    public sealed class RandomDataSource : IDataSource
    {
        private Lazy<RandomNumberGenerator> _rng = new Lazy<RandomNumberGenerator>(() => new RNGCryptoServiceProvider());

        public void GetBytes(byte[] data)
        {
            _rng.Value.GetBytes(data);
        }

        public void Dispose()
        {
            if (_rng.IsValueCreated)
                _rng.Value.Dispose();
        }
    }
}
