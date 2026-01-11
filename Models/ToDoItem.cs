using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace odev3son.Models
{
    public class ToDoItem : INotifyPropertyChanged
    {
        private string _id;
        private string _title;
        private string _description;
        private bool _isCompleted;
        private DateTime _date;
        private TimeSpan _time;

        public string Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FormattedTitle));
                }
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StatusColor));
                    OnPropertyChanged(nameof(StatusText));
                    OnPropertyChanged(nameof(Icon));
                    OnPropertyChanged(nameof(FormattedTitle));
                }
            }
        }

        public DateTime Date
        {
            get => _date;
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FormattedDateTime));
                }
            }
        }

        public TimeSpan Time
        {
            get => _time;
            set
            {
                if (_time != value)
                {
                    _time = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FormattedDateTime));
                }
            }
        }

        // Hesaplanan property'ler
        public string FormattedDateTime => $"{Date:dd.MM.yyyy} {Time:hh\\:mm}";
        public string FormattedTitle => IsCompleted ? $"✓ {Title}" : Title;
        public string StatusText => IsCompleted ? "Tamamlandı" : "Devam Ediyor";
        public Color StatusColor => IsCompleted ? Colors.Green : Colors.Orange;
        public string Icon => IsCompleted ? "✅" : "📝";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ToDoItem()
        {
            Id = Guid.NewGuid().ToString();
            Title = string.Empty;
            Description = string.Empty;
            Date = DateTime.Now;
            Time = DateTime.Now.TimeOfDay;
        }
    }
}