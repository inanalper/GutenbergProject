using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GutenbergProject.Entities
{
    public class UserBook
    {

        public UserBook() { }

        public UserBook(User user, string bookId)
        {
            this.User = user;
            this.bookId = bookId;
        }

        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int userId { get; set; }

        public string bookName { get; set; }
        public string bookImage { get; set; }
        public string bookId { get; set; }
        public int onPage { get; set; }
        public DateTime? lastReaded { get; set; }

        // Navigation property for User
        public virtual User User { get; set; }
    }
}
