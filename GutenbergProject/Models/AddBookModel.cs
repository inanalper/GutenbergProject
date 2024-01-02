using System.ComponentModel.DataAnnotations;

namespace GutenbergProject.Models
{
    public class AddBookModel
    {
        [Required]
        public string bookName { get; set; }
        [Required]
        public string bookImage { get; set; }
        [Required]
        public string bookId { get; set; }
    }
}
