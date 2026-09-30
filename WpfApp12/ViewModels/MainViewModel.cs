using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WpfApp12.Models;
using WpfApp12.Services;

namespace WpfApp12.ViewModels
{
    /// <summary>ViewModel главного окна со списком книг.</summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly ILibraryService _service;
        private Book _selectedBook;
        private string _searchText;
        private string _statusMessage;

        public MainViewModel(ILibraryService service)
        {
            _service = service;

            AddCommand = new RelayCommand(async _ => await AddAsync());
            EditCommand = new RelayCommand(async _ => await EditAsync(), _ => SelectedBook != null);
            DeleteCommand = new RelayCommand(async _ => await DeleteAsync(), _ => SelectedBook != null);
            IssueCommand = new RelayCommand(async _ => await IssueAsync(),
                _ => SelectedBook != null && SelectedBook.Status == BookStatus.Available);
            ReturnCommand = new RelayCommand(async _ => await ReturnAsync(),
                _ => SelectedBook != null && SelectedBook.Status == BookStatus.Issued);
            SearchCommand = new RelayCommand(async _ => await RefreshAsync());
            ResetSearchCommand = new RelayCommand(async _ => { SearchText = string.Empty; await RefreshAsync(); });
        }

        /// <summary>Показ формы редактирования (задаётся слоем View). Возвращает true, если сохранено.</summary>
        public Func<BookEditViewModel, bool?> ShowEditDialog { get; set; }

        /// <summary>Запрос подтверждения (задаётся слоем View).</summary>
        public Func<string, bool> Confirm { get; set; }

        public ObservableCollection<Book> Books { get; } = new ObservableCollection<Book>();

        public Book SelectedBook { get => _selectedBook; set => SetProperty(ref _selectedBook, value); }
        public string SearchText { get => _searchText; set => SetProperty(ref _searchText, value); }
        public string StatusMessage { get => _statusMessage; private set => SetProperty(ref _statusMessage, value); }

        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand IssueCommand { get; }
        public RelayCommand ReturnCommand { get; }
        public RelayCommand SearchCommand { get; }
        public RelayCommand ResetSearchCommand { get; }

        public async Task RefreshAsync()
        {
            await RunAsync(async () =>
            {
                var list = await _service.SearchAsync(SearchText);
                Books.Clear();
                foreach (var book in list) Books.Add(book);
                StatusMessage = "Найдено книг: " + Books.Count;
            });
        }

        private async Task AddAsync()
        {
            var editVm = new BookEditViewModel(_service);
            if (ShowEditDialog?.Invoke(editVm) == true)
            {
                await RefreshAsync();
                StatusMessage = "Книга добавлена.";
            }
        }

        private async Task EditAsync()
        {
            var editVm = new BookEditViewModel(_service, SelectedBook);
            if (ShowEditDialog?.Invoke(editVm) == true)
            {
                await RefreshAsync();
                StatusMessage = "Изменения сохранены.";
            }
        }

        private async Task DeleteAsync()
        {
            var book = SelectedBook;
            if (book == null) return;
            if (Confirm != null && !Confirm("Удалить книгу «" + book.Title + "»?")) return;

            await RunAsync(async () =>
            {
                await _service.DeleteAsync(book.Id);
                await RefreshAsync();
                StatusMessage = "Книга удалена.";
            });
        }

        private async Task IssueAsync()
        {
            var book = SelectedBook;
            if (book == null) return;
            await RunAsync(async () =>
            {
                await _service.IssueAsync(book.Id);
                await RefreshAsync();
                StatusMessage = "Книга выдана.";
            });
        }

        private async Task ReturnAsync()
        {
            var book = SelectedBook;
            if (book == null) return;
            await RunAsync(async () =>
            {
                await _service.ReturnAsync(book.Id);
                await RefreshAsync();
                StatusMessage = "Книга возвращена.";
            });
        }

        private async Task RunAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (InvalidOperationException ex)
            {
                StatusMessage = ex.Message;
            }
            catch (Exception ex)
            {
                StatusMessage = "Ошибка: " + ex.Message;
            }
        }
    }
}
