using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using decelerate.Utils.JWT;

namespace decelerate.Views.UserArea
{
    public class IndexModel
    {
        public IndexModel(JWTPayload jwtPayload)
        {
            this.JWTpayload = jwtPayload;
        }
        public JWTPayload JWTpayload { get; set; }
    }
}
