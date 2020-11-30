using GalaSoft.MvvmLight;

namespace mcv2.MainViewPlugin
{
    public class MetadataViewModel : ViewModelBase
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged();
            }
        }
        private string _elapsed;
        public string Elapsed
        {
            get { return _elapsed; }
            set
            {
                _elapsed = value;
                RaisePropertyChanged();
            }
        }
        private string _currentViewers;
        public string CurrentViewers
        {
            get { return _currentViewers; }
            set
            {
                _currentViewers = value;
                RaisePropertyChanged();
            }
        }
        private string _totalViewers;
        public string TotalViewers
        {
            get { return _totalViewers; }
            set
            {
                _totalViewers = value;
                RaisePropertyChanged();
            }
        }
        private string _active;
        public string Active
        {
            get { return _active; }
            set
            {
                _active = value;
                RaisePropertyChanged();
            }
        }
        private string _others;
        private string _connectionName;

        public string Others
        {
            get { return _others; }
            set
            {
                _others = value;
                RaisePropertyChanged();
            }
        }
        public string ConnectionName
        {
            get
            {
                return _connectionName;
            }
            set
            {
                _connectionName = value;
                RaisePropertyChanged();
            }
        }
        public MetadataViewModel()
        {
            ConnectionName = "";
            Title = "-";
            Elapsed = "-";
            CurrentViewers = "-";
            TotalViewers = "-";
            Active = "-";
            Others = "-";
        }
    }
}
