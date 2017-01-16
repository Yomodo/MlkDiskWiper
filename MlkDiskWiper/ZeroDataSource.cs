namespace MlkDiskWiper
{
    public sealed class ValueDataSource : IDataSource
    {
        readonly byte _value;

        public ValueDataSource(byte value)
        {
            _value = value;
        }

        public void GetBytes(byte[] data)
        {
            for (var i = 0; i < data.Length; i++)
                data[i] = _value;
        }

        public void Dispose()
        {
        }
    }
}
