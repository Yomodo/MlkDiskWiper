using System;
using System.Collections.Generic;
using System.Linq;

namespace MlkDiskWiper
{
    public class DataWipeType
    {
        public static DataWipeType RandomWipe { get; } = new DataWipeType("Write Random Bytes", () => new RandomDataSource());
        public static DataWipeType Zeroes { get; } = new DataWipeType("Write Zeroes", () => new ValueDataSource(0));

        public static IReadOnlyCollection<DataWipeType> All { get; } = new[]
        {
            RandomWipe,
            Zeroes,
        }.ToList().AsReadOnly();

        public string Description { get; private set; }
        public Func<IDataSource> SourceFactory { get; private set; }

        private DataWipeType(string description, Func<IDataSource> sourceFactory)
        {
            Description = description;
            SourceFactory = sourceFactory;
        }
    }
}
