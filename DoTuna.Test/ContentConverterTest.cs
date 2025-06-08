using System;
using System.Collections.Generic;
using Xunit;
using DoTuna;

namespace DoTuna.Test
{
    public class ContentConverterTest
    {
        private ContentConverter GetConverter()
        {
            var threads = new List<JsonIndexDocument> {
                new JsonIndexDocument { threadId = 123, title = "t", username = "u", createdAt = DateTime.Now, updatedAt = DateTime.Now, size = 1 }
            };
            var fileNameMap = new ThreadFileNameMap(threads, "{id}");
            return new ContentConverter(fileNameMap);
        }

        [Theory]
        [InlineData(
            ">>123>>45", 
            "<a href=\"123.html#response_45\">>>123>>45</a>")]
        [InlineData(
            "https://bbs.tunaground.net/trace.php/b/123/45", 
            "<a href=\"123.html#response_45\" target=\"_blank\">https://bbs.tunaground.net/trace.php/b/123/45</a>")]
        [InlineData(
            "https://tunaground.co/card2?post/trace.php/b/123/45", 
            "<a href=\"123.html#response_45\" target=\"_blank\">https://tunaground.co/card2?post/trace.php/b/123/45</a>")]
        [InlineData(
            "https://tunaground.co/card2?post/trace.php\\?bbs=b&amp;card_number=123", 
            "<a href=\"123.html\" target=\"_blank\">https://tunaground.co/card2?post/trace.php\\?bbs=b&amp;card_number=123</a>")]
        [InlineData(
            "https://example.com/abc/123", 
            "<a href=\"https://example.com/abc/123\" target=\"_blank\">https://example.com/abc/123</a>")]
        public void ConvertContent_ReplacesLinks(string input, string expected)
        {
            var converter = GetConverter();
            var thread = new JsonThreadDocument { threadId = 123 };
            var res = new Response { threadId = 123 };
            var output = converter.ConvertContent(input, thread, res);
            Assert.Contains(expected, output);
        }
    }
}
