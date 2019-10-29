using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using decelerate.Utils.JWT;
using decelerate.Models;
using System.ComponentModel.DataAnnotations;

namespace decelerate.Views.UserArea
{
    public class IndexModel
    {
        public User User { get; set; }

        [Required, Range(-100, 100)]
        public int SpeedChoice { get; set; }
    }
}
