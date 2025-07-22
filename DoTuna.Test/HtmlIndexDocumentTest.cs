using DoTuna;
using Xunit;

public class HtmlIndexDocumentTest
{
    [Fact]
    public void FromJson_MapsPropertiesCorrectly()
    {
        // Arrange
        var doc = new JsonIndexDocument
        {
            threadId = 123,
            title = "Title & Test!",
            username = "User\nName"
        };

        var fileNameMap = new ThreadFileNameMap(new[] { doc }, "{id}/{title}");

        // Act
        var htmlDoc = HtmlIndexDocument.FromJson(doc, fileNameMap);

        // Assert
        Assert.Equal("123", htmlDoc.thread_id);
        Assert.Contains("Title", htmlDoc.thread_title); // Escaped 문자열 포함 확인
        Assert.Contains("User", htmlDoc.thread_username); // Escaped 문자열 포함 확인
        Assert.NotEmpty(htmlDoc.file_name);
    }

    [Theory]
    [InlineData("normal", "normal")]
    [InlineData("space here", "space%20here")]
    [InlineData("tab\tchar", "tab%09char")]
    [InlineData("newline\nchar", "newline%0Achar")]
    [InlineData("special!@#", "special%21%40%23")]
    public void EncodeToHref_EncodesCorrectly(string input, string expected)
    {
        // Act
        string encoded = HtmlIndexDocument.EncodeToHref(input);

        // Assert
        Assert.Equal(expected, encoded);
    }

    [Theory]
    [InlineData(' ', true)]
    [InlineData('\n', true)]
    [InlineData('\r', true)]
    [InlineData('\t', true)]
    [InlineData('!', true)]
    [InlineData('@', true)]
    [InlineData('#', true)]
    [InlineData('%', true)]
    [InlineData('^', true)]
    [InlineData('&', true)]
    [InlineData('*', true)]
    [InlineData('(', true)]
    [InlineData(')', true)]
    [InlineData('a', false)]
    [InlineData('Z', false)]
    [InlineData('1', false)]
    [InlineData('-', false)]
    public void IsTargetChar_IdentifiesCorrectly(char c, bool expected)
    {
        // Use reflection to test private method (since IsTargetChar is private)
        var method = typeof(HtmlIndexDocument).GetMethod("IsTargetChar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        bool result = (bool)method.Invoke(null, new object[] { c });

        Assert.Equal(expected, result);
    }
}
