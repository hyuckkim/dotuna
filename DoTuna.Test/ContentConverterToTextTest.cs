using System;
using System.Collections.Generic;
using Xunit;
using DoTuna;

namespace DoTuna.Test
{
    public class ContentConverterToTextTest
    {
        private ContentConverter GetConverter()
        {
            var threads = new List<JsonIndexDocument> {
                new JsonIndexDocument { threadId = 123, title = "t", username = "u", createdAt = DateTime.Now, updatedAt = DateTime.Now, size = 1 }
            };
            var fileNameMap = new ThreadFileNameMap(threads, "{id}");
            return new ContentConverterToText(fileNameMap, "https://testurl.com");
        }

        [Theory]
        // 앵커 변환 테스트
        [InlineData( // >>7
            "&gt;&gt;7", 
            "https://testurl.com/123.html#response_7")]
        [InlineData( // >456>
            "&gt;456&gt;", 
            "https://testurl.com/456.html")]
        [InlineData( // >456>7
            "&gt;456&gt;7", 
            "https://testurl.com/456.html#response_7")]
        [InlineData( // tuna>456>7
            "tuna&gt;456&gt;7", 
            "https://testurl.com/456.html#response_7")]
        [InlineData( // >>7
            ">>7", 
            "https://testurl.com/123.html#response_7")]
        [InlineData( // >456>
            ">456>", 
            "https://testurl.com/456.html")]
        [InlineData( // >456>7
            ">456>7", 
            "https://testurl.com/456.html#response_7")]
        [InlineData( // tuna>456>7
            "tuna>456>7", 
            "https://testurl.com/456.html#response_7")]
        
        // tunaground.net 링크 변환 테스트
        [InlineData(
            "http://bbs.tunaground.net/trace.php/tuna/123/456",
            "https://testurl.com/123.html#response_456")]
        [InlineData(
            "https://bbs.tunaground.net/trace.php/tuna/789/101112",
            "https://testurl.com/789.html#response_101112")]
        [InlineData(
            "여기 링크입니다: http://bbs.tunaground.net/trace.php/tuna/123/456 확인해보세요",
            "여기 링크입니다: https://testurl.com/123.html#response_456 확인해보세요")]
        
        // card2 trace.php 경로 링크 변환 테스트
        [InlineData(
            "http://tunaground.co/card2?post/trace.php/tuna/123/456",
            "https://testurl.com/123.html#response_456")]
        [InlineData(
            "https://tunaground.co/card2?post/trace.php/tuna/789/101112",
            "https://testurl.com/789.html#response_101112")]
            
        // card2 쿼리스트링 링크 변환 테스트
        [InlineData(
            "http://tunaground.co/card2?post/trace.php?bbs=tuna&amp;card_number=123",
            "https://testurl.com/123.html")]
        [InlineData(
            "https://tunaground.co/card2?post/trace.php?bbs=tuna&amp;card_number=456",
            "https://testurl.com/456.html")]
            
        // 일반 링크 변환 안되는거 테스트
        [InlineData(
            "http://example.com",
            "http://example.com")]
        [InlineData(
            "https://example.co.kr/page",
            "https://example.co.kr/page")]
        [InlineData(
            "http://example.com/한글경로",
            "http://example.com/한글경로")]
        [InlineData(
            "유튜브는 제외: https://www.youtube.com/embed/abc123",
            "유튜브는 제외: https://www.youtube.com/embed/abc123")]

        public void ConvertContent_ReplacesLinks(string input, string expected)
        {
            var converter = GetConverter();
            var thread = new JsonThreadDocument { threadId = 123 };
            var res = new Response { threadId = 123 };
            var output = converter.ConvertContent(input, thread.threadId);
            Assert.Contains(expected, output);
        }
    }
}
