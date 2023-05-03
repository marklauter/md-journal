using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.IO
{
    public readonly struct ResourceUri
    {
        public static readonly ResourceUri Empty = new(String.Empty);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ResourceUri WithPath(params string[] paths)
        {
            paths = new string[] { this }.Union(paths).ToArray();
            return new(paths);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ResourceUri WithPath(string path)
        {
            return new(this, path);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ResourceUri(params string[] paths)
        {
            if (paths is null)
            {
                throw new ArgumentNullException(nameof(paths));
            }

            if (!paths.Any())
            {
                throw new ArgumentException($"'{nameof(paths)}' cannot be empty.", nameof(paths));
            }

            this.value = Path.Combine(paths);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ResourceUri(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"'{nameof(value)}' cannot be null or whitespace.", nameof(value));
            }

            this.value = value;
        }

        private readonly string value;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return this.value;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator string(ResourceUri resourceUri)
        {
            return resourceUri.value;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ResourceUri(string value)
        {
            return new ResourceUri(value);
        }
    }
}
