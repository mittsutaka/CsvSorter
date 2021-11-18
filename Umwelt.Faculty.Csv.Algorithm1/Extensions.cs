using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Umwelt.Faculty.Csv.Algorithm1
{
    static class Extensions
    {
        #region Configuration

        public static IEnumerable<string> GetStrings(this IConfiguration configuration, string key)
        {
            var str = configuration[key];
            if (string.IsNullOrWhiteSpace(str)) return Enumerable.Empty<string>();
            return str.Split(',').Select(t => t.Trim());
        }

        #endregion

        #region CSV

        public static CsvReader AsCsvReader(this Stream stream)
        {
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture) { MissingFieldFound = null };
            var reader = new StreamReader(stream);
            return new CsvReader(reader, configuration);
        }

        public static CsvWriter AsCsvWriter(this Stream stream)
        {
            var writer = new StreamWriter(stream
#if DEBUG
                , new UTF8Encoding(true)
#endif
                );
            return new CsvWriter(writer, CultureInfo.InvariantCulture);
        }

        public static void WriteFields(this CsvWriter writer, IEnumerable<string> fields)
        {
            foreach (var field in fields) writer.WriteField(field);
        }

        public static IEnumerable<string> GetFields(this CsvReader reader, IEnumerable<string> names)
        {
            return names.Select(t => reader.GetField(t));
        }

        /// <summary>
        /// 現在の行から、ヘッダの数分のフィールドを取得します。<br />
        /// 現在の行が、ヘッダの数と同じ数のフィールドを持つことが明らかな場合、
        /// もしくは、ヘッダの数にかかわらず全てのフィールドを取得したい場合は、
        /// <see cref="CsvReader.Parser.Record">CsvReader.Parser.Record</see>を使うことが推奨されます。
        /// </summary>
        public static IEnumerable<string> GetFields(this CsvReader reader)
        {
            for (int i = 0; i < reader.HeaderRecord.Length; i++)
            {
                yield return reader[i];
            }
        }

        #endregion

        #region StringTo

        public static DateTime? AsDateTime(this string? s) => DateTime.TryParse(s, out DateTime r) ? r : null;

        public static DateTime? AsDate(this string? s) => s.AsDateTime()?.Date;

        /// <summary>
        /// 実績値を変換する場合はAsDecimalを使うことが推奨されます。<br />
        /// 実績値が小数になっていることが稀にあります。0.5（ダース）みたいな…
        /// </summary>
        public static int? AsInt(this string? s) => int.TryParse(s, out int r) ? r : null;

        /// <summary>
        /// 実績値を変換する場合はAsDecimalを使うことが推奨されます。
        /// </summary>
        public static double? AsDouble(this string? s) => double.TryParse(s, out double r) ? r : null;

        public static decimal? AsDecimal(this string? s) => decimal.TryParse(s, out decimal r) ? r : null;

        #endregion
    }

    static class Csv
    {
        /// <summary>既存のファイルを開き、CsvReaderを生成します。</summary>
        public static CsvReader OpenRead(string path) => File.OpenRead(path).AsCsvReader();

        /// <summary>ファイルを作成、もしくは上書き状態で開き、CsvWriterを生成します。</summary>
        public static CsvWriter Create(string path) => File.Create(path).AsCsvWriter();

        public static string? GenerateNextHeaderName(IEnumerable<string> existedHeaders, string initial, string format)
        {
            var hashSet = new HashSet<string>(existedHeaders);
            if (!hashSet.Contains(initial)) return initial;
            for (int i = 2; i < hashSet.Count + 2; i++)
            {
                var name = string.Format(format, i);
                if (!hashSet.Contains(name)) return name;
            }

            return null;
        }
    }
}
