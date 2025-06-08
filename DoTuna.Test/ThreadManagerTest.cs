using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DoTuna;

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

        private JsonIndexDocument Doc(string title, string username, int id) => new JsonIndexDocument {
            title = title, username = username, threadId = id, createdAt = DateTime.Now, updatedAt = DateTime.Now, size = 1
        };

        [Fact]
        public void Filtered_FiltersByTitleAndAuthor()
        {
            var docs = new List<JsonIndexDocument> {
                Doc("foo", "bar", 1), Doc("foo", "baz", 2), Doc("qux", "bar", 3)
            };
            var mgr = new ThreadManager(new DummyRepo(docs));
            mgr.TitleFilter = "foo";
            mgr.AuthorFilter = "bar";
            var filtered = mgr.Filtered.ToList();
            Assert.Single(filtered);
            Assert.Equal(1, filtered[0].threadId);
        }

        [Fact]
        public void Check_Uncheck_Toggle_Works()
        {
            var doc = Doc("t", "u", 1);
            var mgr = new ThreadManager(new DummyRepo(new List<JsonIndexDocument> { doc }));
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

        [Fact]
        public void Checked_ReturnsCheckedDocsSorted()
        {
            var d1 = Doc("a", "b", 2);
            var d2 = Doc("c", "d", 1);
            var mgr = new ThreadManager(new DummyRepo(new List<JsonIndexDocument> { d1, d2 }));
            mgr.Check(d1);
            mgr.Check(d2);
            var checkedList = mgr.Checked.ToList();
            Assert.Equal(1, checkedList[0].threadId);
            Assert.Equal(2, checkedList[1].threadId);
        }
    }
}
