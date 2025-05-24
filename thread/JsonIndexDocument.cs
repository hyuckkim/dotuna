using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DoTuna.Thread
{
    public class JsonIndexDocument : INotifyPropertyChanged
    {
#pragma warning disable IDE1006 // Naming Styles: Because the JSON properties are in snake_case, we need to disable this rule for the properties.
        public string version { get; set; } = "";
        public string boardId { get; set; } = "";
        public int threadId { get; set; } = 0;
        public string title { get; set; } = "";
        public string username { get; set; } = "";
        public DateTime createdAt { get; set; } = new DateTime(0);
        public DateTime updatedAt { get; set; } = new DateTime(0);
        public int size { get; set; } = 0;
#pragma warning restore IDE1006 // Naming Styles

        public override string ToString()
        {
            return string.Format("{0} /  Username: {1} {2}", title, username, threadId);
        }

        private bool _isCheck = false; // for DataGrid
        public bool IsCheck
        {
            get { return _isCheck; }
            set
            {
                if (_isCheck != value)
                {
                    _isCheck = value;
                    OnPropertyChanged("IsCheck");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
