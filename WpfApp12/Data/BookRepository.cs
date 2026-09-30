using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WpfApp12.Models;

namespace WpfApp12.Data
{
    /// <summary>
    /// Реализация репозитория на EF Core. Для каждой операции создаётся
    /// короткоживущий контекст, поэтому отслеживаемые сущности не «залипают».
    /// </summary>
    public class BookRepository : IBookRepository
    {
        private readonly Func<AppDbContext> _contextFactory;

        public BookRepository(Func<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Book>> GetAllAsync()
        {
            using (var db = _contextFactory())
            {
                return await db.Books.AsNoTracking().OrderBy(b => b.Title).ToListAsync();
            }
        }

        public async Task<Book> GetByIdAsync(int id)
        {
            using (var db = _contextFactory())
            {
                return await db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
            }
        }

        public async Task<List<Book>> SearchAsync(string query)
        {
            using (var db = _contextFactory())
            {
                return await db.Books.AsNoTracking()
                    .Where(b => b.Title.Contains(query) || b.Author.Contains(query))
                    .OrderBy(b => b.Title)
                    .ToListAsync();
            }
        }

        public async Task AddAsync(Book book)
        {
            using (var db = _contextFactory())
            {
                db.Books.Add(book);
                await db.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Book book)
        {
            using (var db = _contextFactory())
            {
                db.Books.Update(book);
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var db = _contextFactory())
            {
                var book = await db.Books.FindAsync(id);
                if (book == null) return;
                db.Books.Remove(book);
                await db.SaveChangesAsync();
            }
        }
    }
}
