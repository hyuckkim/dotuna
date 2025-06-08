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
            "&gt;&gt;7", 
            "<a href=\"123.html#response_7\">&gt;&gt;7</a>")]
        [InlineData(
            "&gt;456&gt;", 
            "<a href=\"456.html#response_\">&gt;456&gt;</a>")]
        [InlineData(
            "&gt;456&gt;&gt;7", 
            "<a href=\"456.html#response_7\">&gt;456&gt;&gt;7</a>")]
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
