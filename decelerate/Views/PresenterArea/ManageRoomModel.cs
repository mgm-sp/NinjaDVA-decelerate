using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using decelerate.Models;

namespace decelerate.Views.PresenterArea
{
    public class ManageRoomModel
    {
        public ManageRoomModel() { }

        public ManageRoomModel(Room room)
        {
            Room = room;
            Name = room.Name;
            Public = room.Public;
        }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public bool Public { get; set; }

        public Room Room { get; set; }
    }
}
