namespace GutenbergPresentation.Models
{
    public class AuthorViewModel
    {
        public string Name { get; set; }
        public int BirthYear { get; set; }
        public int DeathYear { get; set; }
    }
  

    public class BookViewModel
    {




        public int Id { get; set; }


        public string Title { get; set; }
        public List<AuthorViewModel> Authors { get; set; }
        public List<string> Subjects { get; set; }
        public List<string> Bookshelves { get; set; }
        public List<string> Languages { get; set; }

        public Dictionary<string, string> Formats { get; set; }
        public int DownloadCount { get; set; }

    }
    public class BookReadModel
    {
        public int count { get; set; }
        public string next { get; set; }

        public string previous { get; set; }

        public List<Dictionary<string, string>> results { get; set; }

    }
    public class BookResultViewModel
    {
        public int Count { get; set; }
        public object Next { get; set; }
        public object Previous { get; set; }
        public List<BookViewModel> Results { get; set; }
    }

}
