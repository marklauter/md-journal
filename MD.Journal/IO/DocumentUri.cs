using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.IO
{
    public sealed class DocumentUri
    {
        public DocumentUri(string path, string resourceName)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            if (String.IsNullOrWhiteSpace(resourceName))
            {
                throw new ArgumentException($"'{nameof(resourceName)}' cannot be null or whitespace.", nameof(resourceName));
            }

            this.value = System.IO.Path.Combine(path, resourceName);
        }

        private DocumentUri(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"'{nameof(value)}' cannot be null or whitespace.", nameof(value));
            }

            this.value = value;
        }

        private readonly string value;

        public string? Path => System.IO.Path.GetDirectoryName(this.value);
        public string ResourceName => System.IO.Path.GetFileName(this.value);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return this.value;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator string(DocumentUri documentUri)
        {
            return documentUri.value;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DocumentUri(string value)
        {
            return new DocumentUri(value);
        }
    }
}
