using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace decelerate.Models
{
    public class Room
    {
        [Key,Required]
        public int Id { get; set; }

        [Required,MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public bool Public { get; set; }

        [Required,MaxLength(50)]
        public string AdmissionCode { get; set; }

        [Required,ForeignKey("Presenter")]
        public string PresenterName { get; set; }
        public virtual Presenter Presenter { get; set; }

        public string DisplayText
        {
            get
            {
                return $"{Name} (by {PresenterName})";
            }
        }

        public virtual ICollection<User> Users { get; set; }
    }
}
