using System.Collections.Generic;
using System.Threading.Tasks;
using WpfApp12.Models;

namespace WpfApp12.Services
{
    /// <summary>Бизнес-операции библиотеки.</summary>
    public interface ILibraryService
    {
        Task<List<Book>> GetAllAsync();
        Task<List<Book>> SearchAsync(string query);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(int id);
        Task IssueAsync(int id);
        Task ReturnAsync(int id);
    }
}
