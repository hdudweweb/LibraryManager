namespace WpfApp12.Models
{
    /// <summary>Сущность «Книга» (Domain Model).</summary>
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public BookStatus Status { get; set; }
    }
}
