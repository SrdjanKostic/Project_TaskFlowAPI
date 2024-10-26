using System.ComponentModel.DataAnnotations;

namespace TaskFlowAPI.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(256)]
        public string PasswordHash { get; set; }
        public string Role { get; set; }

        //Konstruktor koji postavlja podrazumevane vrednosti ( resava warning Non-nullable) :
        public User()
        {
            Username = string.Empty;
            PasswordHash = string.Empty;
            Role = string.Empty;
        }
    }

}
