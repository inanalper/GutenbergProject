using System.ComponentModel.DataAnnotations;

namespace GutenbergProject.Entities
{
    public class User
    {
        public User(string email, string userName,  string passwordHash)
        {
            this.email = email;
            this.userName = userName;
            this.passwordHash = passwordHash;
           

        }

        [Key]
        public int id { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public string passwordHash { get; set; }

        public void registerUser()
        {

        }

    }
}
