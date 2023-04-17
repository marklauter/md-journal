using MD.Journal;
using Newtonsoft.Json;
using System.Text;

var lines = new string[]
{
    "## heading 2",
    "line 1",
    "line 2",
    String.Empty,
    "line 3",
    "- bullet 1",
    "- bullet 2",
    String.Empty,
    "line 1",
    "line 2",
    String.Empty,
    "line 3",
};

var entry = JournalEntryBuilder.Create()
    .WithTitle("title")
    .WithAuthor("author")
    .WithSummary("summary")
    .WithBody(String.Join(Environment.NewLine, lines))
    .WithTags(new string[] { "tag1", "tag2" })
    .Build();

var journal = new Journal(".");
await journal.WriteAsync(entry, CancellationToken.None);

Console.WriteLine();
Console.WriteLine(JsonConvert.SerializeObject(entry, Formatting.Indented));
Console.WriteLine();
Console.WriteLine("------------------");
Console.WriteLine();
using var md = entry.ToMarkdownStream();
Console.WriteLine(Encoding.UTF8.GetString(md.GetBuffer()[3..((int)md.Length - 1)]));
Console.WriteLine();
Console.WriteLine();
