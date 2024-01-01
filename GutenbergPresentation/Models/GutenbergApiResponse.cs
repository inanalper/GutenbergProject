namespace GutenbergPresentation.Models
{
    public class GutenbergApiResponse
    {
        public int Count { get; set; }
        public string Next { get; set; }
        public string Previous { get; set; }
        public List<BookResult> Results { get; set; }
    }

    public class BookResult
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Author> Authors { get; set; }
        public List<string> Subjects { get; set; }
    }

    public class Author
    {
        public string Name { get; set; }
        public int BirthYear { get; set; }
        public int DeathYear { get; set; }
    }


}
