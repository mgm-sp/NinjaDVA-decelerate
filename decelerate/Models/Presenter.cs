using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace decelerate.Models
{
    public class Presenter
    {
        [Key,Required,MaxLength(50)]
        public string Name { get; set; }

        [Required,MaxLength(100)]
        public string PasswordHash { get; set; }

        public ICollection<Room> Rooms { get; set; }
    }
}
