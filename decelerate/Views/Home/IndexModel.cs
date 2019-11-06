using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using decelerate.Models;

namespace decelerate.Views.Home
{
    public class IndexModel
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }

        public IEnumerable<Room> PublicRoomList { get; set; }

        public int RoomId { get; set; }

        [MaxLength(50)]
        public string RoomCode { get; set; }
    }
}
