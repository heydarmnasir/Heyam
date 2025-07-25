using System.ComponentModel;

namespace BE.ViewModel
{
    public class LawInfo : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        private string _approvalDate;
        public string ApprovalDate
        {
            get => _approvalDate;
            set { _approvalDate = value; OnPropertyChanged(nameof(ApprovalDate)); }
        }

        private string _documentType;
        public string DocumentType
        {
            get => _documentType;
            set { _documentType = value; OnPropertyChanged(nameof(DocumentType)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
