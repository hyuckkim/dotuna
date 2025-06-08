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
        // 앵커 변환 테스트
        [InlineData( // >>7
            "&gt;&gt;7", 
            "<a href=\"123.html#response_7\">&gt;&gt;7</a>")]
        [InlineData( // >456>
            "&gt;456&gt;", 
            "<a href=\"456.html#response_\">&gt;456&gt;</a>")]
        [InlineData( // >456>7
            "&gt;456&gt;7", 
            "<a href=\"456.html#response_7\">&gt;456&gt;7</a>")]
        [InlineData( // tuna>456>7
            "tuna&gt;456&gt;7", 
            "<a href=\"456.html#response_7\">&gt;456&gt;7</a>")]
        
        // tunaground.net 링크 변환 테스트
        [InlineData(
            "http://bbs.tunaground.net/trace.php/tuna/123/456",
            "<a href=\"123.html#response_456\" target=\"_blank\">http://bbs.tunaground.net/trace.php/tuna/123/456</a>")]
        [InlineData(
            "https://bbs.tunaground.net/trace.php/tuna/789/101112",
            "<a href=\"789.html#response_101112\" target=\"_blank\">https://bbs.tunaground.net/trace.php/tuna/789/101112</a>")]
        [InlineData(
            "여기 링크입니다: http://bbs.tunaground.net/trace.php/tuna/123/456 확인해보세요",
            "여기 링크입니다: <a href=\"123.html#response_456\" target=\"_blank\">http://bbs.tunaground.net/trace.php/tuna/123/456</a> 확인해보세요")]
        
        // card2 trace.php 경로 링크 변환 테스트
        [InlineData(
            "http://tunaground.co/card2?post/trace.php/tuna/123/456",
            "<a href=\"123.html#response_456\" target=\"_blank\">http://tunaground.co/card2?post/trace.php/tuna/123/456</a>")]
        [InlineData(
            "https://tunaground.co/card2?post/trace.php/tuna/789/101112",
            "<a href=\"789.html#response_101112\" target=\"_blank\">https://tunaground.co/card2?post/trace.php/tuna/789/101112</a>")]
            
        // card2 쿼리스트링 링크 변환 테스트
        [InlineData(
            "http://tunaground.co/card2?post/trace.php?bbs=tuna&amp;card_number=123",
            "<a href=\"123.html\" target=\"_blank\">http://tunaground.co/card2?post/trace.php?bbs=tuna&amp;card_number=123</a>")]
        [InlineData(
            "https://tunaground.co/card2?post/trace.php?bbs=tuna&amp;card_number=456",
            "<a href=\"456.html\" target=\"_blank\">https://tunaground.co/card2?post/trace.php?bbs=tuna&amp;card_number=456</a>")]
            
        // 일반 링크 변환 테스트
        [InlineData(
            "http://example.com",
            "<a href=\"http://example.com\" target=\"_blank\">http://example.com</a>")]
        [InlineData(
            "https://example.co.kr/page",
            "<a href=\"https://example.co.kr/page\" target=\"_blank\">https://example.co.kr/page</a>")]
        [InlineData(
            "http://example.com/한글경로",
            "<a href=\"http://example.com/한글경로\" target=\"_blank\">http://example.com/한글경로</a>")]
        [InlineData(
            "유튜브는 제외: https://www.youtube.com/embed/abc123",
            "유튜브는 제외: https://www.youtube.com/embed/abc123")]

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
