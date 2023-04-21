using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.Tags
{
    public readonly struct TagId
    {
        private readonly string value;

        private TagId(string tag)
        {
            if (String.IsNullOrEmpty(tag))
            {
                throw new ArgumentException($"'{nameof(tag)}' cannot be null or empty.", nameof(tag));
            }

            this.value = tag
                .ToLowerInvariant()
                .Replace(" ", "-");
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return this.value;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator string(TagId tagId)
        {
            return tagId.value;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator TagId(string tag)
        {
            return new TagId(tag);
        }
    }
}
