using System.ComponentModel.DataAnnotations;

namespace GutenbergProject.Models
{
    public class UserSignInModel
    {

        [Required]
       
        public string email { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
     
    }
}
