using System;
using System.Collections.Generic;
using System.Linq;
using DoTuna.Features.Export;
using Xunit;

namespace DoTuna.Test
{
    public class ThreadManagerTest
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
            var mgr = new ThreadSelectionHelper(new DummyRepo(docs));
            mgr.TitleFilter = "foo";
            mgr.AuthorFilter = "bar";
            var filtered = mgr.Filtered.ToList();
            Assert.Single(filtered);
            Assert.Equal(1UL, filtered[0].threadId);
        }

        [Fact]
        public void Check_Uncheck_Toggle_Works()
        {
            var doc = Doc("t", "u", 1);
            var mgr = new ThreadSelectionHelper(new DummyRepo(new List<JsonIndexDocument> { doc }));
            Assert.False(mgr.IsChecked(doc));
            mgr.Check(doc);
            Assert.True(mgr.IsChecked(doc));
            mgr.Uncheck(doc);
            Assert.False(mgr.IsChecked(doc));
            mgr.Toggle(doc);
            Assert.True(mgr.IsChecked(doc));
            mgr.Toggle(doc);
            Assert.False(mgr.IsChecked(doc));
        }
    }
}
