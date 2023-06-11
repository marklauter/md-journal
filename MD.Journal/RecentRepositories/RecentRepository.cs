using Newtonsoft.Json;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.RecentRepositories;

public readonly record struct RecentRepository(
    string Path,
    DateTime LastAccessUtc)
    : IComparable<RecentRepository>
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(RecentRepository other)
    {
        return String.Compare(this.Path, other.Path, StringComparison.OrdinalIgnoreCase);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(RecentRepository left, RecentRepository right)
    {
        return left.CompareTo(right) < 0;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(RecentRepository left, RecentRepository right)
    {
        return left.CompareTo(right) <= 0;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(RecentRepository left, RecentRepository right)
    {
        return left.CompareTo(right) > 0;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(RecentRepository left, RecentRepository right)
    {
        return left.CompareTo(right) >= 0;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator RecentRepository(string json)
    {
        return JsonConvert.DeserializeObject<RecentRepository>(json);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator string(RecentRepository recentItem)
    {
        return JsonConvert.SerializeObject(recentItem);
    }

    [JsonIgnore]
    public string LastAccessLocal => this.LastAccessUtc.ToLocalTime().ToShortDateString();
}

