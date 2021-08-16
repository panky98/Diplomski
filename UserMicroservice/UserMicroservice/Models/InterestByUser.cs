using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.Models
{
    public class InterestByUser
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public int InterestId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("InterestId")]
        public Interest Interest { get; set; }

    }
}
