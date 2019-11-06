using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using decelerate.Models;

namespace decelerate.Views.PresenterArea
{
    public class IndexModel
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public bool Public { get; set; }

        public Presenter Presenter { get; set; }
    }
}
