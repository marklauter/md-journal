using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;

namespace MD.Journal.IO.Indexes
{
    internal readonly struct PropertyId
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private PropertyId(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"'{nameof(value)}' cannot be null or whitespace.", nameof(value));
            }

            this.value = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
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
        public static implicit operator string(PropertyId propertyId)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(propertyId.value));
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator PropertyId(string value)
        {
            return new PropertyId(value);
        }
    }
}
