using System.Collections.Generic;
using System.Linq;

namespace DoTuna.Features.Export
{
    public class ThreadSelectionHelper
    {
        private readonly IIndexRepository _indexRepository;

        private HashSet<JsonIndexDocument> _checked = new HashSet<JsonIndexDocument>();

        private string _titleFilter = string.Empty;
        private string _authorFilter = string.Empty;

        private List<JsonIndexDocument> _filteredCache = new List<JsonIndexDocument>();

        public IEnumerable<JsonIndexDocument> All => _allCache ??= _indexRepository.Get().ToList();
        private List<JsonIndexDocument>? _allCache = null;

        public IEnumerable<JsonIndexDocument> Checked => _checked;
        public bool IsChecked(JsonIndexDocument doc) => _checked.Contains(doc);

        public IEnumerable<JsonIndexDocument> Filtered => _filteredCache;

        public string TitleFilter
        {
            get => _titleFilter;
            set
            {
                if (_titleFilter != value)
                {
                    _titleFilter = value;
                    RefreshFiltered();
                }
            }
        }

        public string AuthorFilter
        {
            get => _authorFilter;
            set
            {
                if (_authorFilter != value)
                {
                    _authorFilter = value;
                    RefreshFiltered();
                }
            }
        }

        public ThreadSelectionHelper(IIndexRepository indexRepository)
        {
            _indexRepository = indexRepository;
            RefreshFiltered(); // 초기 필터링
        }

        private void RefreshFiltered()
        {
            var baseList = All;

            if (!string.IsNullOrEmpty(_titleFilter))
                baseList = baseList.Where(x => x.title.Contains(_titleFilter));

            if (!string.IsNullOrEmpty(_authorFilter))
                baseList = baseList.Where(x => x.username.Contains(_authorFilter));

            _filteredCache = baseList.ToList();
        }

        public void Check(JsonIndexDocument doc, bool isCheck)
        {
            if (doc == null) return;

            if (isCheck)
                _checked.Add(doc);
            else
                _checked.Remove(doc);
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
                _checked.Remove(doc);
            else
                _checked.Add(doc);
        }
    }
}