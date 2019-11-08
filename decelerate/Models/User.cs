using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace decelerate.Models
{
    public class User
    {
        [Required,MaxLength(50)]
        public string Name { get; set; }

        [Required,ForeignKey("Room")]
        public int RoomId { get; set; }
        [JsonIgnore]
        public virtual Room Room { get; set; }
        public bool ShouldSerializeRoom() { return false; }
        /* ^- Thanks to this method, SignalR won't serialize the Room property,
         * preventing a loop User -> Room -> User -> Room -> ... */

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
