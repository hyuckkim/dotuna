using Xunit;
using System.Collections.Generic;
using DoTuna.Core;

public class TemplateFormatterTest
{
  [Fact]
  public void Format_ReturnsEmpty_WhenTemplateIsEmpty()
  {
    var values = new Dictionary<string, string> { { "key", "value" } };
    Assert.Equal(string.Empty, TemplateFormatter.Format("", values));
  }

  [Fact]
  public void Format_ReplacesSimplePlaceholder()
  {
    var values = new Dictionary<string, string> { { "name", "Alice" } };
    string template = "Hello {name}!";
    Assert.Equal("Hello Alice!", TemplateFormatter.Format(template, values));
  }

  [Fact]
  public void Format_UsesFallback_WhenValueMissing()
  {
    var values = new Dictionary<string, string>();
    string template = "Hello {name:Unknown}!";
    Assert.Equal("Hello Unknown!", TemplateFormatter.Format(template, values));
  }

  [Fact]
  public void Format_UsesFallback_WhenValueIsEmpty()
  {
    var values = new Dictionary<string, string> { { "name", "" } };
    string template = "Hello {name:Unknown}!";
    Assert.Equal("Hello Unknown!", TemplateFormatter.Format(template, values));
  }

  [Fact]
  public void Format_RemovesInvalidFileNameChars()
  {
    var values = new Dictionary<string, string> { { "file", "inva|id:name" } };
    string template = "File: {file}";
    var result = TemplateFormatter.Format(template, values);
    Assert.DoesNotContain("|", result);
    Assert.DoesNotContain(":", result);
  }

  [Fact]
  public void Format_TruncatesFrontAndBack_WithOmitString()
  {
    var values = new Dictionary<string, string> { { "text", "abcdefghij" } };
    string template = "{text 3...3}";
    Assert.Equal("abc...hij", TemplateFormatter.Format(template, values));
  }

  [Fact]
  public void Format_TruncatesFrontOnly_WithOmitString()
  {
    var values = new Dictionary<string, string> { { "text", "abcdefghij" } };
    string template = "{text 5...}";
    Assert.Equal("abcde...", TemplateFormatter.Format(template, values));
  }

  [Fact]
  public void Format_TruncatesBackOnly_WithOmitString()
  {
    var values = new Dictionary<string, string> { { "text", "abcdefghij" } };
    string template = "{text ...4}";
    Assert.Equal("...ghij", TemplateFormatter.Format(template, values));
  }

  [Fact]
  public void Format_ReturnsInput_WhenTruncateCountsExceedLength()
  {
    var values = new Dictionary<string, string> { { "text", "abc" } };
    string template = "{text 2...2}";
    Assert.Equal("abc", TemplateFormatter.Format(template, values));
  }

  [Fact]
  public void Format_ReturnsInput_WhenNoTruncateCounts()
  {
    var values = new Dictionary<string, string> { { "text", "abc" } };
    string template = "{text ...}";
    Assert.Equal("abc", TemplateFormatter.Format(template, values));
  }

  [Fact]
  public void Format_MultiplePlaceholders()
  {
    var values = new Dictionary<string, string> { { "a", "1" }, { "b", "2" } };
    string template = "{a} and {b}";
    Assert.Equal("1 and 2", TemplateFormatter.Format(template, values));
  }

  [Fact]
  public void Format_EmptyValueAndNoFallback_ReturnsEmpty()
  {
    var values = new Dictionary<string, string> { { "x", "" } };
    string template = "Value: {x}";
    Assert.Equal("Value: ", TemplateFormatter.Format(template, values));
  }
}