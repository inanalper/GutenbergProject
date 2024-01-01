using System.ComponentModel.DataAnnotations;

namespace GutenbergProject.Models
{

    public class BookModel
    {
        [Required]
        public string bookName { get; set; }
        [Required]
        public string bookImage { get; set; }
        [Required]
        public string bookId { get; set; }
        [Required]
        public int onPage { get; set; }
    }
}
