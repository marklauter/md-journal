using MD.Journal.IO.Readers;
using MD.Journal.IO.Writers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.IO.Indexes
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIndex<TValue>(
            this IServiceCollection services)
            where TValue : IComparable<TValue>
        {
            _ = services
                .AddResourceReader()
                .AddResourceWriter();

            return services
                .AddTransient<Func<string, string, IIndex<TValue>>>(serviceProvider =>
                    (path, name) => serviceProvider.CreateNamedIndex<TValue>(path, name));
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IIndex<TValue> GetNamedIndex<TValue>(
            this IServiceProvider serviceProvider,
            string path,
            string name)
            where TValue : IComparable<TValue>
        {
            return serviceProvider.GetRequiredService<Func<string, string, IIndex<TValue>>>()(path, name);
        }

        private static IIndex<TValue> CreateNamedIndex<TValue>(
            this IServiceProvider serviceProvider,
            string path,
            string name)
            where TValue : IComparable<TValue>
        {
            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
            }

            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            var reader = serviceProvider.GetRequiredService<IResourceReader>();
            var writer = serviceProvider.GetRequiredService<IResourceWriter>();
            var options = Options.Create(new IndexOptions { Name = name, Path = path });
            var logger = serviceProvider.GetRequiredService<ILogger<Index>>();
            return new Index<TValue>(
                reader,
                writer,
                options,
                logger);
        }

        public static IServiceCollection AddPropertyGraphIndex(
            this IServiceCollection services)
        {
            _ = services
                .AddResourceReader()
                .AddResourceWriter();

            return services.AddTransient<Func<string, string, IPropertyGraphIndex>>(
                serviceProvider =>
                (path, name) => serviceProvider.CreateNamedPropertyGraphIndex(path, name));
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IPropertyGraphIndex GetNamedPropertyGraphIndex(
            this IServiceProvider serviceProvider,
            string path,
            string name)
        {
            return serviceProvider.GetRequiredService<Func<string, string, IPropertyGraphIndex>>()(path, name);
        }

        private static IPropertyGraphIndex CreateNamedPropertyGraphIndex(
            this IServiceProvider serviceProvider,
            string path,
            string name)
        {
            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
            }

            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            var reader = serviceProvider.GetRequiredService<IResourceReader>();
            var writer = serviceProvider.GetRequiredService<IResourceWriter>();
            var options = Options.Create(new IndexOptions { Name = name, Path = path });
            var logger = serviceProvider.GetRequiredService<ILogger<PropertyGraphIndex>>();
            return new PropertyGraphIndex(
                reader,
                writer,
                options,
                logger);
        }
    }
}
