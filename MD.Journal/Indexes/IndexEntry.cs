using Newtonsoft.Json;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.Indexes
{
    public readonly record struct IndexEntry<TValue>(string Key, TValue Value)
        : IComparable<IndexEntry<TValue>>
        where TValue : IComparable<TValue>
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(IndexEntry<TValue> other)
        {
            return String.Compare(this.Key, other.Key, StringComparison.OrdinalIgnoreCase);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator IndexEntry<TValue>(string json)
        {
            return JsonConvert.DeserializeObject<IndexEntry<TValue>>(json);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator string(IndexEntry<TValue> entry)
        {
            return JsonConvert.SerializeObject(entry);
        }
    }
}
