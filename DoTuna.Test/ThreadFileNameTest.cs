using System;
using DoTuna.Core;
using Xunit;

namespace DoTuna.Test
{
    public class ThreadFileNameTest
    {
        private JsonIndexDocument SampleDoc => new JsonIndexDocument
        {
            threadId = 1234,
            title = "제목",
            username = "작성자",
            createdAt = new DateTime(2023, 1, 1),
            updatedAt = new DateTime(2023, 2, 2),
            size = 42
        };

        [Fact]
        public void Constructor_FromDocAndPattern_CreatesExpectedFileName()
        {
            var pattern = "{id}/{title}";
            var name = new ThreadFile(SampleDoc, pattern);
            Assert.Equal("1234/제목.html", name.FileName);
            Assert.Equal("1234", name.FileNameSegments[0]);
            Assert.Equal("제목.html", name.FileNameSegments[1]);
        }

        [Fact]
        public void Constructor_WithSegments_SetsFieldsCorrectly()
        {
            var threadId = 5678UL;
            var segments = new[] { "a", "b", "c.html" };
            var name = new ThreadFile(threadId, segments);
            Assert.Equal(threadId, name.ThreadId);
            Assert.Equal("a/b/c.html", name.FileName);
        }

        [Fact]
        public void GetRelativePathTo_OtherInSameFolder_ReturnsSimpleName()
        {
            var from = new ThreadFile(1, new[] { "a", "file1.html" });
            var to = new ThreadFile(2, new[] { "a", "file2.html" });

            var rel = from.GetRelativePathTo(to);
            Assert.Equal("file2.html", rel);
        }

        [Fact]
        public void GetRelativePathTo_OtherInDeeperFolder_ReturnsRelativePath()
        {
            var from = new ThreadFile(1, new[] { "a", "file.html" });
            var to = new ThreadFile(2, new[] { "a", "b", "c.html" });

            var rel = from.GetRelativePathTo(to);
            Assert.Equal("b/c.html", rel);
        }

        [Fact]
        public void GetRelativePathTo_OtherInParentFolder_ReturnsDotDots()
        {
            var from = new ThreadFile(1, new[] { "a", "b", "file.html" });
            var to = new ThreadFile(2, new[] { "a", "target.html" });

            var rel = from.GetRelativePathTo(to);
            Assert.Equal("../target.html", rel);
        }

        [Fact]
        public void GetRelativePathToRoot_Works()
        {
            var t = new ThreadFile(1, new[] { "a", "b", "c", "d.html" });
            Assert.Equal("../../..", t.GetRelativePathToRoot());
        }

        [Fact]
        public void Equality_WorksCorrectly()
        {
            var a = new ThreadFile(1, new[] { "a", "b.html" });
            var b = new ThreadFile(1, new[] { "a", "b.html" });
            var c = new ThreadFile(2, new[] { "a", "b.html" });
            var d = new ThreadFile(1, new[] { "a", "c.html" });

            Assert.True(a == b);
            Assert.False(a == c);
            Assert.False(a == d);
            Assert.True(a.Equals(b));
        }

        [Fact]
        public void GetTemplateName_TruncatesProperly()
        {
            var doc = SampleDoc;
            var template = "{title}/{title}/{title}/{title}/{title}";
            var result = ThreadFile.GetTemplateName(doc, template);
            Assert.True(result.Length <= 200);
        }
    }
}
