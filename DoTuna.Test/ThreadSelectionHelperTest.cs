// File: Features/Export/List/ThreadSelectionHelperTest.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace DoTuna.Features.Export
{
    public class ThreadSelectionHelperTest
    {
        private class DummyRepo : IIndexRepository
        {
            private readonly List<JsonIndexDocument> _docs;
            public DummyRepo(List<JsonIndexDocument> docs) { _docs = docs; }
            public List<JsonIndexDocument> Get() => _docs;
        }

        private JsonIndexDocument Doc(string title, string username, ulong id) => new JsonIndexDocument {
            title = title, username = username, threadId = id, createdAt = DateTime.Now, updatedAt = DateTime.Now, size = 1
        };

        [Fact]
        public void Filtered_FiltersByTitleAndAuthor()
        {
            var docs = new List<JsonIndexDocument> {
                Doc("foo", "bar", 1), Doc("foo", "baz", 2), Doc("qux", "bar", 3)
            };
            var helper = new ThreadSelectionHelper(new DummyRepo(docs));
            helper.TitleFilter = "foo";
            helper.AuthorFilter = "bar";
            var filtered = helper.Filtered.ToList();
            Assert.Single(filtered);
            Assert.Equal(1UL, filtered[0].threadId);
        }

        [Fact]
        public void Check_Uncheck_Toggle_Works()
        {
            var doc = Doc("t", "u", 1);
            var helper = new ThreadSelectionHelper(new DummyRepo(new List<JsonIndexDocument> { doc }));
            Assert.False(helper.IsChecked(doc));
            helper.Check(doc);
            Assert.True(helper.IsChecked(doc));
            helper.Uncheck(doc);
            Assert.False(helper.IsChecked(doc));
            helper.Toggle(doc);
            Assert.True(helper.IsChecked(doc));
            helper.Toggle(doc);
            Assert.False(helper.IsChecked(doc));
        }

        [Fact]
        public void Filter_Performance_IsAcceptable()
        {
            // Create 100_000 docs, half with "match" in title, half with "user" in username
            var docs = new List<JsonIndexDocument>();
            for (int i = 0; i < 100_000; i++)
            {
                docs.Add(Doc(
                    i % 2 == 0 ? "match" : "other",
                    i % 2 == 1 ? "user" : "someone",
                    (ulong)i
                ));
            }
            var helper = new ThreadSelectionHelper(new DummyRepo(docs));

            var sw = Stopwatch.StartNew();
            helper.TitleFilter = "match";
            helper.AuthorFilter = "someone";
            var filtered = helper.Filtered.ToList();
            sw.Stop();

            // Should be about 50_000 items
            Assert.Equal(50_000, filtered.Count);
            // Filtering should complete within 500ms
            Assert.True(sw.ElapsedMilliseconds < 500, $"Filtering took too long: {sw.ElapsedMilliseconds}ms");
        }
    }
}