using Newtonsoft.Json;
using System.Collections.Immutable;

namespace MD.Journal
{
    public readonly record struct TagGraph
    {
        private readonly ImmutableDictionary<string, ImmutableArray<string>> taggedDocuments = ImmutableDictionary<string, ImmutableArray<string>>.Empty;

        public TagGraph MapDocument(string tag, string documentId)
        {
            if (String.IsNullOrWhiteSpace(documentId))
            {
                throw new ArgumentException($"'{nameof(documentId)}' cannot be null or whitespace.", nameof(documentId));
            }

            var documents = this
                .Documents(tag)
                .Add(documentId);

            return new TagGraph(this.taggedDocuments.SetItem(tag, documents));
        }

        public ImmutableArray<string> Documents(string tag)
        {
            if (String.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentException($"'{nameof(tag)}' cannot be null or whitespace.", nameof(tag));
            }

            if (!this.taggedDocuments.TryGetValue(tag, out var documents))
            {
                documents = ImmutableArray<string>.Empty;
            }

            return documents;
        }

        public TagGraph() { }

        private TagGraph(ImmutableDictionary<string, ImmutableArray<string>> taggedDocuments)
        {
            this.taggedDocuments = taggedDocuments;
        }

        public static TagGraph FromJson(string json)
        {
            if (String.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException($"'{nameof(json)}' cannot be null or whitespace.", nameof(json));
            }

            var taggedDocuments = JsonConvert.DeserializeObject<ImmutableDictionary<string, ImmutableArray<string>>>(json);
            taggedDocuments ??= ImmutableDictionary<string, ImmutableArray<string>>.Empty;
            return new TagGraph(taggedDocuments);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this.taggedDocuments);
        }
    }
}
