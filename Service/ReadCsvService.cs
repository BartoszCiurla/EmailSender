using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace Service
{
    public interface IReadCsvService<out T>
    {
        IEnumerable<T> ReadCsv();
    }
    public class ReadCsvService<T> : IReadCsvService<T>, IDisposable
    {
        private readonly ICsvReader _csvReader;

        public ReadCsvService(ICsvReader csvReader)
        {
            _csvReader = csvReader;
        }

        public static int RecordCountToTake = 100;
        public IEnumerable<T> ReadCsv()
        {
            int counter = 0;
            while (_csvReader.Read())
            {
                if (counter == RecordCountToTake)
                    break;
                counter++;
                yield return (_csvReader.GetRecord<T>());
            }
        }

        public void Dispose()
        {
            _csvReader.Dispose();
        }
    }
}
