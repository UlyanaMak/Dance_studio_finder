using DanceStudioFinder.Models;

namespace DanceStudioFinder.ViewModels
{
    public class CreatePricesStudioViewModel
    {
        public Admin Admin { get; set; }
        public DanceStudio DanceStudio { get; set; }
        public List<Price> Prices { get; set;} = new List<Price>();
    }
}
