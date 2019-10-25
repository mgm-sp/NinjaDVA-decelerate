using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace decelerate.Utils.JWT
{
    public class JWTHeader
    {
        public JWTHeader(string typ, string alg)
        {
            this.typ = typ;
            this.alg = alg;
        }
        public string typ { get; set; }
        public string alg { get; set; }
    }
}
