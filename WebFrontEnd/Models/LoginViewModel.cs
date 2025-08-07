using System.ComponentModel.DataAnnotations;

namespace WebFrontEnd.Models
{
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string names { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
