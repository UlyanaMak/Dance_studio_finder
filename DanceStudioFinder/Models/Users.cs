using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DanceStudioFinder.Models
{
    public class Users: IdentityUser 
    {
        public string Name { get; set; }
    }
}
