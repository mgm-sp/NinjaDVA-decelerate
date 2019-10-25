using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace decelerate.Utils.JWT
{
    public class JWTPayload
    {
        public JWTPayload(string name)
        {
            this.name = name;
        }
        public string name { get; set; }
    }
}
