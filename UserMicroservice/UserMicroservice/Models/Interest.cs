using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.Models
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
