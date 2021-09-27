using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models.UserMicroservice
{
    public class InterestByUser
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public int InterestId { get; set; }

        [JsonIgnore]
        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("InterestId")]
        public Interest Interest { get; set; }

    }
}
