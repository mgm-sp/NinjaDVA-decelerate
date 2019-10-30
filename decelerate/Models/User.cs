using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace decelerate.Models
{
    public class User
    {
        [Key]
        public string Name { get; set; }

        public int? SpeedChoice { get; set; }

        public int? TransformedSpeedChoice
        {
            get
            {
                if (SpeedChoice == null)
                {
                    return null;
                }
                return (int)((100 + SpeedChoice ?? 0) / 2.0);
            }
        }

        public DateTime LastAction { get; set; }
    }
}
