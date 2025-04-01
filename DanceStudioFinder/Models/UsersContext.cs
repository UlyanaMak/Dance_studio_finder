using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DanceStudioFinder.Models
{
    public class UsersContext: IdentityDbContext<Users>
    {
        public UsersContext(DbContextOptions options) : base(options) 
        { 
        }
    }
}
