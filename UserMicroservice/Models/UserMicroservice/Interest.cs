using System.ComponentModel.DataAnnotations;

namespace Models.UserMicroservice
{
    public class Interest
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public Interest()
        {

        }

        public Interest(string name)
        {
            this.Name = name;
        }
    }
}
