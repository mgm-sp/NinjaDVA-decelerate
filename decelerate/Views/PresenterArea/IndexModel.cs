using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using decelerate.Models;

namespace decelerate.Views.PresenterArea
{
    public class IndexModel
    {
        public IEnumerable<User> Users { get; set; }
    }
}
