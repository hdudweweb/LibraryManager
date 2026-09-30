using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WpfApp12.Data;
using WpfApp12.Models;

namespace WpfApp12.Services
{
    /// <summary>
    /// Бизнес-правила. Нарушения правил сообщаются через InvalidOperationException
    /// с текстом, пригодным для показа пользователю.
    /// </summary>
    public class LibraryService : ILibraryService
    {
        private readonly IBookRepository _repository;

        public LibraryService(IBookRepository repository)
        {
            _repository = repository;
        }

        public Task<List<Book>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<List<Book>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return _repository.GetAllAsync();
            return _repository.SearchAsync(query.Trim());
        }

        public Task AddAsync(Book book)
        {
            Validate(book);
            book.Title = book.Title.Trim();
            book.Author = book.Author.Trim();
            return _repository.AddAsync(book);
        }

        public async Task UpdateAsync(Book book)
        {
            Validate(book);
            var existing = await _repository.GetByIdAsync(book.Id);
            if (existing == null)
                throw new InvalidOperationException("Книга не найдена.");

            book.Title = book.Title.Trim();
            book.Author = book.Author.Trim();
            await _repository.UpdateAsync(book);
        }

        public async Task DeleteAsync(int id)
        {
            var book = await GetExistingAsync(id);
            if (book.Status == BookStatus.Issued)
                throw new InvalidOperationException("Нельзя удалить выданную книгу. Сначала оформите возврат.");
            await _repository.DeleteAsync(id);
        }

        public async Task IssueAsync(int id)
        {
            var book = await GetExistingAsync(id);
            if (book.Status == BookStatus.Issued)
                throw new InvalidOperationException("Книга уже выдана.");
            book.Status = BookStatus.Issued;
            await _repository.UpdateAsync(book);
        }

        public async Task ReturnAsync(int id)
        {
            var book = await GetExistingAsync(id);
            if (book.Status == BookStatus.Available)
                throw new InvalidOperationException("Книга уже находится в библиотеке.");
            book.Status = BookStatus.Available;
            await _repository.UpdateAsync(book);
        }

        private async Task<Book> GetExistingAsync(int id)
        {
            var book = await _repository.GetByIdAsync(id);
            if (book == null)
                throw new InvalidOperationException("Книга не найдена.");
            return book;
        }

        private static void Validate(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));
            if (string.IsNullOrWhiteSpace(book.Title))
                throw new InvalidOperationException("Укажите название книги.");
            if (string.IsNullOrWhiteSpace(book.Author))
                throw new InvalidOperationException("Укажите автора книги.");
            if (book.Year < 1 || book.Year > DateTime.Now.Year)
                throw new InvalidOperationException("Год издания должен быть от 1 до " + DateTime.Now.Year + ".");
        }
    }
}
