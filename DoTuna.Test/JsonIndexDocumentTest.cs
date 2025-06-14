using System;
using Xunit;
using DoTuna;

namespace DoTuna.Test
{
    public class JsonIndexDocumentTest
    {
        [Fact]
        public void GetTemplateName_ReplacesAllPlaceholders()
        {
            var doc = new JsonIndexDocument
            {
                threadId = 42,
                title = "TestTitle",
                username = "TestUser",
                createdAt = new DateTime(2024, 6, 1),
                updatedAt = new DateTime(2024, 6, 2),
                size = 123
            };

            string template = "{id}_{title}_{name}_{created}_{updated}_{size}";
            string expected = "42_TestTitle_TestUser_2024-06-01_2024-06-02_123";
            string result = doc.getTemplateName(template);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetTemplateName_RemovesInvalidFileNameChars()
        {
            var doc = new JsonIndexDocument
            {
                threadId = 1,
                title = "Ti:t*le|?",
                username = "User<>",
                createdAt = DateTime.Now,
                updatedAt = DateTime.Now,
                size = 1
            };

            string template = "{title}_{name}";
            string result = doc.getTemplateName(template);

            Assert.DoesNotContain(":", result);
            Assert.DoesNotContain("*", result);
            Assert.DoesNotContain("|", result);
            Assert.DoesNotContain("?", result);
            Assert.DoesNotContain("<", result);
            Assert.DoesNotContain(">", result);
            Assert.Contains("Title_User", result);
        }

        [Fact]
        public void GetTemplateName_TruncatesLongResult()
        {
            var doc = new JsonIndexDocument
            {
                threadId = 1,
                title = new string('A', 300),
                username = "User",
                createdAt = DateTime.Now,
                updatedAt = DateTime.Now,
                size = 1
            };

            string template = "{title}";
            string result = doc.getTemplateName(template);

            Assert.True(result.Length <= 200);
        }

        [Fact]
        public void GetTemplateName_EmptyTemplate_ReturnsEmpty()
        {
            var doc = new JsonIndexDocument();
            string result = doc.getTemplateName("");
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetTemplateName_TruncatesWithEllipsisOrPrefix()
        {
            var doc = new JsonIndexDocument
            {
                threadId = 1,
                title = "ABCDEFGHIJKLMNO",     // 15자
                username = "ZXYWVUTSRQPONMLK", // 16자
                createdAt = new DateTime(2024, 6, 1),
                updatedAt = new DateTime(2024, 6, 1),
                size = 0
            };

            string template = "{title 10..}_{name _6}";
            string result = doc.getTemplateName(template);

            // 예상 결과:
            // title 10.. → "ABCDEFGHIJ.." (앞 10자 + 생략 "..")
            // 구분자 _
            // name _6   → "_ONMLK"       (뒤 6자 + 접두 "_")
            string expected = "ABCDEFGHIJ..__ONMLK";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetTemplateName_TruncatesWithDoubleSideOmit()
        {
            var doc = new JsonIndexDocument
            {
                threadId = 1,
                title = "ABCDEFGHIJKLMNOPQRSTUVWXYZ", // 26자
                username = "UserNameExample",             // 15자
                createdAt = new DateTime(2024, 6, 1),
                updatedAt = new DateTime(2024, 6, 1),
                size = 0
            };

            string template = "{title 5__5}_{name 4..4}";
            string result = doc.getTemplateName(template);

            // 예상 결과:
            // title 5__5 → "ABCDE__VWXYZ" (앞5자 + "__" + 뒤5자)
            // name 4..4  → "User..mple"  (앞4자 + ".." + 뒤4자)
            string expected = "ABCDE__VWXYZ_User..mple";

            Assert.Equal(expected, result);
        }
    }
}