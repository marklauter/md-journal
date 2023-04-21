using Newtonsoft.Json;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.Recents
{
    public readonly record struct RecentItem(string Key, DateTime LastAccessUtc)
        : IComparable<RecentItem>
    {
        [JsonIgnore]
        public string LastAccessLocal => this.LastAccessUtc.ToLocalTime().ToShortDateString();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(RecentItem other)
        {
            return String.Compare(this.Key, other.Key, StringComparison.OrdinalIgnoreCase);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(RecentItem left, RecentItem right)
        {
            return left.CompareTo(right) < 0;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(RecentItem left, RecentItem right)
        {
            return left.CompareTo(right) <= 0;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(RecentItem left, RecentItem right)
        {
            return left.CompareTo(right) > 0;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(RecentItem left, RecentItem right)
        {
            return left.CompareTo(right) >= 0;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator RecentItem(string json)
        {
            return JsonConvert.DeserializeObject<RecentItem>(json);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator string(RecentItem recentItem)
        {
            return JsonConvert.SerializeObject(recentItem);
        }
    }
}
