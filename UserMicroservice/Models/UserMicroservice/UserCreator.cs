using System;
using System.Collections.Generic;
using System.Text;

namespace Models.UserMicroservice
{
    public class UserCreator
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public IList<int> InterestsByUser { get; set; }
    }
}
