//using Grynwald.MarkdownGenerator;

//namespace MD.Journal.IO.Internal
//{
//    internal abstract class ResourceWriter
//        : IResourceWriter
//    {
//        public ResourceWriter(string path, string resourceName)
//        {
//            if (String.IsNullOrWhiteSpace(path))
//            {
//                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
//            }

//            if (String.IsNullOrWhiteSpace(resourceName))
//            {
//                throw new ArgumentException($"'{nameof(resourceName)}' cannot be null or whitespace.", nameof(resourceName));
//            }

//            this.Uri = System.IO.Path.Combine(path, resourceName);
//            this.Path = path;
//            this.ResourceName = resourceName;
//        }

//        public string Path { get; }
//        public string ResourceName { get; }
//        public string Uri { get; }

//        public abstract Task AppendLineAsync(string value);
//        public abstract Task AppendTextAsync(string value);
//        public abstract Task OverwriteAllLinesAsync(IEnumerable<string> lines);
//        public abstract Task OverwriteAllTextAsync(string value);
//        public abstract Task<IEnumerable<string>> ReadAllLinesAsync();
//        public abstract Task<IEnumerable<string>> ReadLinesAsync(Pagination pagination);
//        public abstract Task<string> ReadTextAsync();
//    }


//    internal abstract class MemoryResourceStore
//        : IResource
//    {
//        public MemoryResourceStore(string path, string resourceName)
//        {
//            if (String.IsNullOrWhiteSpace(path))
//            {
//                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
//            }

//            if (String.IsNullOrWhiteSpace(resourceName))
//            {
//                throw new ArgumentException($"'{nameof(resourceName)}' cannot be null or whitespace.", nameof(resourceName));
//            }

//            this.Uri = System.IO.Path.Combine(path, resourceName);
//            this.Path = path;
//            this.ResourceName = resourceName;
//        }

//        public string Path { get; }
//        public string ResourceName { get; }
//        public string Uri { get; }

//        public abstract Task AppendLineAsync(string value);
//        public abstract Task AppendTextAsync(string value);
//        public abstract Task OverwriteAllLinesAsync(IEnumerable<string> lines);
//        public abstract Task OverwriteAllTextAsync(string value);
//        public abstract Task<IEnumerable<string>> ReadAllLinesAsync();
//        public abstract Task<IEnumerable<string>> ReadLinesAsync(Pagination pagination);
//        public abstract Task<string> ReadTextAsync();

//        public void Save(string path)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
