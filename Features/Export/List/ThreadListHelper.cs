using System;
using System.Linq;
using System.Windows.Forms;

namespace DoTuna.Features.Export
{
    public class ThreadListHelper
    {
        private readonly ThreadSelectionHelper _threadManager;
        private readonly DataGridView _grid;
        private readonly CheckBox _selectAllCheckBox;
        private readonly TextBox _titleFilterInput;
        private readonly TextBox _authorFilterInput;

        public ThreadListHelper(
            ThreadSelectionHelper threadManager,
            DataGridView grid,
            CheckBox selectAllCheckBox,
            TextBox titleFilterInput,
            TextBox authorFilterInput)
        {
            _threadManager = threadManager ?? throw new ArgumentNullException(nameof(threadManager));
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
            _selectAllCheckBox = selectAllCheckBox ?? throw new ArgumentNullException(nameof(selectAllCheckBox));
            _titleFilterInput = titleFilterInput ?? throw new ArgumentNullException(nameof(titleFilterInput));
            _authorFilterInput = authorFilterInput ?? throw new ArgumentNullException(nameof(authorFilterInput));

            // 이벤트 등록
            _grid.CellValueNeeded += OnCellValueNeeded;
            _grid.CellClick += OnGridCellClick;
            _selectAllCheckBox.CheckedChanged += OnSelectAllChanged;
            _titleFilterInput.TextChanged += (s, e) => ApplyTitleFilter(_titleFilterInput.Text);
            _authorFilterInput.TextChanged += (s, e) => ApplyAuthorFilter(_authorFilterInput.Text);

            RefreshGrid();
        }

        public void ApplyTitleFilter(string filter)
        {
            _grid.RowCount = 0;
            _threadManager.TitleFilter = filter;
            RefreshGrid();
        }

        public void ApplyAuthorFilter(string filter)
        {
            _grid.RowCount = 0;
            _threadManager.AuthorFilter = filter;
            RefreshGrid();
        }

        public void RefreshGrid()
        {
            _grid.RowCount = _threadManager.Filtered.Count();
            _grid.Invalidate();
            UpdateSelectAllCheckBox();
        }

        private void UpdateSelectAllCheckBox()
        {
            _selectAllCheckBox.CheckedChanged -= OnSelectAllChanged;
            _selectAllCheckBox.Checked = _threadManager.Filtered.All(d => _threadManager.IsChecked(d));
            _selectAllCheckBox.CheckedChanged += OnSelectAllChanged;
        }

        private void OnCellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _threadManager.Filtered.Count())
                return;

            var doc = _threadManager.Filtered.ElementAt(e.RowIndex);

            switch (_grid.Columns[e.ColumnIndex].Name)
            {
                case "IsCheck":
                    e.Value = _threadManager.IsChecked(doc);
                    break;
                case "ThreadName":
                    e.Value = doc.title;
                    break;
                case "UserName":
                    e.Value = doc.username;
                    break;
            }
        }

        private void OnGridCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _threadManager.Filtered.Count())
                return;

            _threadManager.Toggle(_threadManager.Filtered.ElementAt(e.RowIndex));
            UpdateSelectAllCheckBox();
            _grid.Invalidate();
        }

        private void OnSelectAllChanged(object sender, EventArgs e)
        {
            if (_selectAllCheckBox.Checked)
            {
                foreach (var doc in _threadManager.Filtered)
                    _threadManager.Check(doc);
            }
            else
            {
                foreach (var doc in _threadManager.All)
                    _threadManager.Uncheck(doc);
            }
            _grid.Invalidate();
        }
    }
}
