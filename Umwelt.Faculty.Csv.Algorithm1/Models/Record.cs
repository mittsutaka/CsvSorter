using CsvHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Umwelt.Faculty.Csv.Algorithm1.Models
{
    /// <summary>
    /// CSVの1行を表現します。
    /// </summary>
    record Record(DateTime Date, string[] Keys, object?[] Fields);

    static class RecordExtensions
    {
        public static Record? GetRecord(this CsvReader reader, string dateHeader, IEnumerable<string> keyHeaders, IEnumerable<string> numHeaders)
        {
            if (reader[dateHeader].AsDate() is not DateTime date) return null;
            var keys = reader.GetFields(keyHeaders).ToArray();
            var fields = Enumerable.Range(0, reader.HeaderRecord.Length).Select<int, object?>(t => reader.HeaderRecord[t] switch
            {
                string header when numHeaders.Contains(header) => reader[t].AsDecimal(),
                _ => reader[t],
            }).ToArray();

            return new Record(date, keys, fields);
        }
    }
}
