using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace decelerate.Views.PresenterArea
{
    public class RegisterModel
    {
        [Required, MaxLength(50)]
        public string Username { get; set; }

        [Required, DataType(DataType.Password), MinLength(8, ErrorMessage = "The password has to be at least eight characters long.")]
        public string Password { get; set; }

        [Required, DataType(DataType.Password)]
        [Display(Name = "Repeat password")]
        [Compare("Password")]
        public string PasswordRepeat { get; set; }
    }
}
