using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;

namespace Service
{
    public interface IReadCsvService<out T>
    {
        IEnumerable<T> ReadCsv(int currentRow);
    }
    public class ReadCsvService<T> : IReadCsvService<T>, IDisposable
    {
        private readonly ICsvReader _csvReader;
        private readonly int _recordsToTake;

        public ReadCsvService(ICsvReader csvReader, int recordsToTake)
        {
            _csvReader = csvReader;
            _recordsToTake = recordsToTake;
        }

        public IEnumerable<T> ReadCsv(int currentRow)
        {
            return _csvReader.GetRecords<T>().Skip(currentRow).Take(_recordsToTake).AsEnumerable();
        }

        public void Dispose()
        {
            _csvReader.Dispose();
        }
    }
}
