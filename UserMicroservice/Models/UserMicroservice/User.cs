using BCrypt.Net;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.UserMicroservice
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        
        public IList<InterestByUser> InterestsByUser { get; set; }

        public User()
        {
            this.InterestsByUser = new List<InterestByUser>();
        }

        public User(string name,string surname,string number,string username,string password)
        {
            this.Name = name;
            this.Surname = surname;
            this.PhoneNumber = number;
            this.Username = username;
            this.Password = User.HashPassword(password);
        }

        public static string HashPassword(string pass)
        {
            return BCrypt.Net.BCrypt.HashPassword(pass);
        }
    }
}
