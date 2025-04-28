namespace DanceStudioFinder.Models
{
    public class UserViewModel
    {
        public RegisterViewModel Register { get; set; } = new RegisterViewModel();
        public LoginViewModel Login { get; set; }
        public List<DanceStudio> DanceStudios { get; set; }
    }
}
