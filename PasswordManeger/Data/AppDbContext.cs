using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PasswordManeger.Models;

namespace PasswordManeger.Data
{
    public class AppDbContext : IdentityDbContext<Users>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<UserPasswords> UserPasswords { get; set; }
    }
}
