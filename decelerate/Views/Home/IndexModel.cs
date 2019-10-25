using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace decelerate.Views.Home
{
    public class IndexModel
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }
    }
}
