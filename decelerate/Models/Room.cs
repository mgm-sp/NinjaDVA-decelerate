using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace decelerate.Models
{
    public class Room
    {
        [Key,Required]
        public int Id { get; set; }

        [Required,MaxLength(50)]
        public string Name { get; set; }

        public string DisplayText
        {
            get
            {
                return $"#{Id} - {Name}";
            }
        }

        public ICollection<User> Users { get; set; }
    }
}
