using System.ComponentModel.DataAnnotations;

namespace GutenbergProject.Entities
{
    public class User
    {
        public User() { }

        [Key]
        public int id { get; set; }
        public string userName { get; set; }
        public string passwordHash { get; set; }

        public void registerUser()
        {

        }

    }
}
