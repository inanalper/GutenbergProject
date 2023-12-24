using System.ComponentModel.DataAnnotations;

namespace GutenbergProject.Models
{
    public class UserLoginModel
    {
        [Required]
        public string userName { get; set; }
        public string password { get; set; }
      
    }
}
