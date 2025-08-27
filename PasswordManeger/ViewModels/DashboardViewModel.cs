using System.ComponentModel.DataAnnotations;
using PasswordManeger.Models;
using System.Collections.Generic;

namespace PasswordManeger.ViewModels
{
    public class DashBoardViewModel
    {
        public IEnumerable<UserPasswords> Passwords { get; set; } 
        public AddPasswordViewModel NewPassword { get; set; }
    }
}
