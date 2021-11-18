using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Umwelt.Faculty.Csv.Algorithm1.Models
{
    class StringArrayEqualityComparer : IEqualityComparer<string[]>
    {
        public static readonly StringArrayEqualityComparer Default = new();

        public bool Equals(string[]? x, string[]? y)
        {
            if (x is null || y is null) throw new NullReferenceException();
            return Enumerable.SequenceEqual(x, y);
        }

        public int GetHashCode([DisallowNull] string[] obj)
        {
            HashCode hash = default;
            for (int i = 0; i < obj.Length; i++) hash.Add(obj[i]);
            return hash.ToHashCode();
        }
    }

    class DateStringArrayEqualityComparer : IEqualityComparer<(DateTime Date, string[] Keys)>
    {
        public static readonly DateStringArrayEqualityComparer Default = new();

        public bool Equals((DateTime Date, string[] Keys) x, (DateTime Date, string[] Keys) y)
        {
            if (x.Date != y.Date) return false;
            return Enumerable.SequenceEqual(x.Keys, y.Keys);
        }

        public int GetHashCode([DisallowNull] (DateTime Date, string[] Keys) obj)
        {
            return HashCode.Combine(obj.Date, StringArrayEqualityComparer.Default.GetHashCode(obj.Keys));
        }
    }
}
