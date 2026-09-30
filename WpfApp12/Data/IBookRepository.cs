using System.Collections.Generic;
using System.Threading.Tasks;
using WpfApp12.Models;

namespace WpfApp12.Data
{
    /// <summary>Абстракция над EF Core: CRUD-операции с книгами (асинхронные).</summary>
    public interface IBookRepository
    {
        Task<List<Book>> GetAllAsync();
        Task<Book> GetByIdAsync(int id);
        Task<List<Book>> SearchAsync(string query);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(int id);
    }
}
