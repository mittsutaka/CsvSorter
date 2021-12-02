using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Umwelt.Faculty.Csv.Algorithm1.Models;

namespace Umwelt.Faculty.Csv.Algorithm1
{
    class Faculty
    {
        private readonly string _inputPath;
        private readonly string _outputPath;
        private string[] _columnNames;
        private string[] _sortColomns;
        private List<string> _sortTypesString;
        private List<SortType> _sortTypes;

        public Faculty(IConfiguration configuration)
        {
            (_inputPath, _outputPath) = Initializer.InFileOutFile(configuration);
            var incar = configuration.GetSection("INCAR");

            // ここで設定を読み取ります。
            _columnNames = incar["TargetHeaders"].Split(',');
            _sortColomns = incar["SortColumns"].Split(',');
            _sortTypesString = incar["SortTypes"].Split(',').ToList();
            _sortTypes = new List<SortType>();
            _sortTypesString.ForEach(t =>
            {
                if (Enum.TryParse(typeof(SortType), t, out var res))
                {
                    if (res is not null)
                    {
                        _sortTypes.Add((SortType)res);
                    }
                }
            });
        }

        public async Task ExecuteAsync()
        {
            // ここにアルゴリズムの処理を書きます。

            using var reader = Csv.OpenRead(_inputPath);
            using var writer = Csv.Create(_outputPath);
            reader.Read();
            reader.ReadHeader();
            var sortIndexes = new List<int>();
            foreach (var sortColumn in _sortColomns)
            {
                var sortIndex = reader.GetFieldIndex(sortColumn);
                sortIndexes.Add(sortIndex);
            }

            writer.WriteFields(_columnNames);
            writer.NextRecord();

            var records = new List<Record>();
            while (reader.Read())
            {
                var record = reader.GetRecord("order_date", _columnNames, new[] { "count" });
                if (record is null) continue;
                records.Add(record);
            }

            var recordCompare = new RecordCompare(sortIndexes, _sortTypes);

            records.Sort(recordCompare);

            foreach (var record in records)
            {
                foreach (var value in record.Keys)
                {
                    writer.WriteField(value);
                }
                writer.NextRecord();
            }
        }
        class RecordCompare : IComparer<Record>
        {
            private List<int> _indexes;
            private List<SortType> _sortTypes;

            public RecordCompare(List<int> indexes, List<SortType> sortTypes)
            {
                _indexes = indexes;
                _sortTypes = sortTypes;
            }

            public int Compare(Record? x, Record? y)
            {
                var j = 0;
                if (x is null || y is null) return 0;
                foreach (var index in _indexes)
                {
                    if (index != 0) j++;
                    if (_sortTypes[j] == SortType.String)
                    {
                        var xValue = x.Keys[index];
                        var yValue = y.Keys[index];
                        var res = string.Compare(xValue, yValue);
                        if (res == 0)
                        {
                            continue;
                        }
                        else
                        {
                            var regex = new Regex(@"\d+|\D+");
                            var xSplit = regex.Matches(xValue).Select(t => t.Value).ToList();
                            var ySplit = regex.Matches(yValue).Select(t => t.Value).ToList();

                            var count = xSplit.Count;
                            if (xSplit.Count > ySplit.Count) count = ySplit.Count;

                            for (var c = 0; c < count; c++)
                            {
                                if (xSplit[c] == ySplit[c])
                                {
                                    continue;
                                }
                                else
                                {
                                    if (double.TryParse(xSplit[c], out var num1) && double.TryParse(ySplit[c], out var num2))
                                    {
                                        return num1 > num2 ? 1 : -1;
                                    }
                                    else
                                    {
                                        return string.Compare(xSplit[c], ySplit[c]);
                                    }
                                }
                            }

                            return res;
                        }
                    }
                    else if (_sortTypes[j] == SortType.Number)
                    {
                        var xValue = int.Parse(x.Keys[index]);
                        var yValue = int.Parse(y.Keys[index]);
                        if (xValue == yValue)
                        {
                            continue;
                        }
                        else
                        {
                            return xValue > yValue ? 1 : -1;
                        }
                    }
                    else if (_sortTypes[j] == SortType.Date)
                    {
                        var xValue = DateTime.Parse(x.Keys[index]);
                        var yValue = DateTime.Parse(y.Keys[index]);
                        var res = DateTime.Compare(xValue, yValue);
                        if (res == 0)
                        {
                            continue;
                        }
                        else
                        {
                            return res;
                        }
                    }
                }

                return 0;
            }
        }

        private enum SortType
        {
            Number,
            String,
            Date
        }
    }
}
