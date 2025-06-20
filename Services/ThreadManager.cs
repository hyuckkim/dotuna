using System.Collections.Generic;
using System.Linq;

namespace DoTuna
{
    public class ThreadManager
    {
        private IIndexRepository _indexRepository;
        private HashSet<JsonIndexDocument> _checked = new HashSet<JsonIndexDocument>();

        private List<JsonIndexDocument>? _titleCache = null;
        private HashSet<JsonIndexDocument>? _authorCache = null;
        private string _titleFilter = string.Empty;
        private string _authorFilter = string.Empty;

        public IEnumerable<JsonIndexDocument> All => _indexRepository.Get();
        public IEnumerable<JsonIndexDocument> Checked => _checked.OrderBy(x => x.threadId);
        public bool IsChecked(JsonIndexDocument doc) => _checked.Contains(doc);
        public IEnumerable<JsonIndexDocument> Filtered {
            get {
                _titleCache ??= All.Where(x => x.title.Contains(_titleFilter)).ToList();
                _authorCache ??= All.Where(x => x.username.Contains(_authorFilter)).ToHashSet();
                return _authorCache.Where(x => _titleCache.Contains(x));
            }
        }

        public string TitleFilter {
            get => _titleFilter;
            set {
                if (_titleFilter != value) {
                    _titleFilter = value;
                    _titleCache = null;
                }
            }
        }
        public string AuthorFilter {
            get => _authorFilter;
            set {
                if (_authorFilter != value) {
                    _authorFilter = value;
                    _authorCache = null;
                }
            }
        }

        public ThreadManager(IIndexRepository indexRepository)
        {
            _indexRepository = indexRepository;
        }
        public void Check(JsonIndexDocument doc, bool isCheck)
        {
            if (doc == null) return;

            if (isCheck)
            {
                _checked.Add(doc);
            }
            else
            {
                _checked.Remove(doc);
            }
        }
        public void Check(JsonIndexDocument doc)
        {
            if (doc == null) return;

            _checked.Add(doc);
        }
        public void Uncheck(JsonIndexDocument doc)
        {
            if (doc == null) return;

            _checked.Remove(doc);
        }
        public void Toggle(JsonIndexDocument doc)
        {
            if (doc == null) return;

            if (_checked.Contains(doc))
            {
                _checked.Remove(doc);
            }
            else
            {
                _checked.Add(doc);
            }
        }
    }
}
