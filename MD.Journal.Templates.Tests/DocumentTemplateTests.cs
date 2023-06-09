namespace MD.Journal.Templates.Tests;

public class DocumentTemplateTests
{
    private const string Content = "Hello, {{name}}.";

    [Fact]
    public void CTOR_Builds_Field_Collection()
    {
        var template = new DocumentTemplate(Content);

        Assert.NotEmpty(template.Fields);
        Assert.Contains("name", template.Fields);
    }

    [Fact]
    public void CTOR_Builds_FieldValue_Dictionary()
    {
        var template = new DocumentTemplate(Content);

        Assert.NotEmpty(template.Values);
        Assert.Contains("name", template.Values.Select(kvp => kvp.Key));
    }

    [Fact]
    public void WriteValue_Mutates_Values_Dictionary()
    {
        var expectedValue = "world";
        var template = new DocumentTemplate(Content);

        template.Write("name", expectedValue);

        Assert.Contains(expectedValue, template.Read("name"));
        Assert.Contains(expectedValue, template.Values.Select(kvp => kvp.Value));
    }

    [Fact]
    public void ToString_Returns_Merged_Document()
    {
        var template = new DocumentTemplate(Content);

        template.Write("name", "world");

        var rendered = template.Render();

        Assert.Equal("Hello, world.", rendered);
    }

    [Fact]
    public void WriteValue_Throws_KeyNotFoundException()
    {
        var expectedKey = "xxx";
        var template = new DocumentTemplate(Content);

        var ex = Assert.Throws<KeyNotFoundException>(() => template.Write(expectedKey, "yyy"));

        Assert.Equal(expectedKey, ex.Message);
    }

    [Fact]
    public void ReadValue_Throws_KeyNotFoundException()
    {
        var expectedKey = "xxx";
        var template = new DocumentTemplate(Content);

        var ex = Assert.Throws<KeyNotFoundException>(() => template.Read(expectedKey));

        Assert.Equal(expectedKey, ex.Message);
    }
}
