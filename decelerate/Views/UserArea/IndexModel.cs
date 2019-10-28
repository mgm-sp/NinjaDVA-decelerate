using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using decelerate.Utils.JWT;
using System.ComponentModel.DataAnnotations;

namespace decelerate.Views.UserArea
{
    public class IndexModel
    {
        public IndexModel() {}

        public IndexModel(JWTPayload jwtPayload)
        {
            this.JWTpayload = jwtPayload;
        }

        public JWTPayload JWTpayload { get; set; }

        [Required, Range(-100, 100)]
        public int SpeedChoice { get; set; }
    }
}
