using System;
using System.Threading.Tasks;
using WpfApp12.Models;
using WpfApp12.Services;

namespace WpfApp12.ViewModels
{
    /// <summary>ViewModel формы добавления/редактирования книги.</summary>
    public class BookEditViewModel : ViewModelBase
    {
        private readonly ILibraryService _service;
        private readonly int _id;
        private readonly bool _isNew;

        private string _title;
        private string _author;
        private string _yearText;
        private BookStatus _status;
        private string _errorMessage;

        public BookEditViewModel(ILibraryService service, Book book = null)
        {
            _service = service;
            _isNew = book == null;

            if (_isNew)
            {
                _yearText = DateTime.Now.Year.ToString();
                _status = BookStatus.Available;
            }
            else
            {
                _id = book.Id;
                _title = book.Title;
                _author = book.Author;
                _yearText = book.Year.ToString();
                _status = book.Status;
            }

            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            CancelCommand = new RelayCommand(_ => CloseRequested?.Invoke(false));
        }

        /// <summary>Запрос закрытия окна: true — сохранено, false — отмена.</summary>
        public event Action<bool> CloseRequested;

        public string WindowTitle => _isNew ? "Добавление книги" : "Редактирование книги";

        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public string Author { get => _author; set => SetProperty(ref _author, value); }
        public string YearText { get => _yearText; set => SetProperty(ref _yearText, value); }
        public BookStatus Status { get => _status; set => SetProperty(ref _status, value); }
        public string ErrorMessage { get => _errorMessage; private set => SetProperty(ref _errorMessage, value); }

        public BookStatus[] Statuses { get; } = (BookStatus[])Enum.GetValues(typeof(BookStatus));

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private async Task SaveAsync()
        {
            ErrorMessage = null;

            int year;
            if (!int.TryParse(YearText, out year))
            {
                ErrorMessage = "Год издания должен быть числом.";
                return;
            }

            var book = new Book
            {
                Id = _id,
                Title = Title,
                Author = Author,
                Year = year,
                Status = Status
            };

            try
            {
                if (_isNew) await _service.AddAsync(book);
                else await _service.UpdateAsync(book);
                CloseRequested?.Invoke(true);
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Ошибка сохранения: " + ex.Message;
            }
        }
    }
}
