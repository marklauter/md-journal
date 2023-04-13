using DevJournal.Markdown;
using Grynwald.MarkdownGenerator;
using System.Text;
using System.Text.Json;

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

var entry = new JournalEntry(
    "title",
    "author",
    String.Join(Environment.NewLine, lines),
    new string[] { "tag1", "tag2" });

var mdTitle = new MdHeading(1, entry.Title);
var mdByLine = new MdEmphasisSpan($"By {entry.Author}, {entry.Date:G}");
var mdTags = new MdTextSpan($"{Environment.NewLine}{String.Join(' ', entry.Tags)}");
var mdHeader = new MdContainerBlock(mdTitle, new MdParagraph(mdByLine, mdTags));

var mdContent = new MdContainerBlock();
for (var i = 0; i < entry.Paragraphs.Length; i++)
{
    mdContent.Add(new MdParagraph(new MdRawMarkdownSpan(entry.Paragraphs[i])));
}

var document = new MdDocument(mdHeader, mdContent);
using var stream = new MemoryStream();
document.Save(stream);

using var file = new FileStream($"{entry.Id}.md", FileMode.OpenOrCreate, FileAccess.Write);
stream.CopyTo(file);
file.Flush();

Console.WriteLine();
Console.WriteLine(JsonSerializer.Serialize(entry, new JsonSerializerOptions { WriteIndented = true }));
Console.WriteLine();
Console.WriteLine("------------------");
Console.WriteLine();
Console.WriteLine(Encoding.UTF8.GetString(stream.ToArray().AsSpan(3..)));
Console.WriteLine();
Console.WriteLine();
