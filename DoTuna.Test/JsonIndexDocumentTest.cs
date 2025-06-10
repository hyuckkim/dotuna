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
            Assert.True(result.Contains("Title_User"));
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
        public void GetTemplateName_NullTemplate_ReturnsEmpty()
        {
            var doc = new JsonIndexDocument();
            string result = doc.getTemplateName(null);
            Assert.Equal(string.Empty, result);
        }
    }
}