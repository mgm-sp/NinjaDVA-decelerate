using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace decelerate.Models
{
    public class Room
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string DisplayText
        {
            get
            {
                return $"#{Id} - {Name}";
            }
        }
    }
}
