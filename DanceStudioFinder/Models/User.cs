using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace DanceStudioFinder.Models
{
    public class User : IdentityUser<int>
    {       
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
