using Microsoft.AspNetCore.Identity;

namespace PasswordManeger.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }

        public List<UserPasswords> passwords { get; set; }
    }
}
