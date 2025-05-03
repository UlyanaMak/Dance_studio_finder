namespace DanceStudioFinder.Models
{
    public class AdminStudioViewModel
    {
        public Admin Admin {  get; set; }
        public DanceStudio DanceStudio { get; set; }
        public Address Address { get; set; }
        public Price Price { get; set; }
        public List<Price> Prices { get; set; }  //список цен
        public DanceGroup DanceGroup { get; set; }
        public AgeLimit AgeLimit { get; set; }
        public Style Style { get; set; }
        public List<Style> Styles { get; set; }  //стили для выбора пользователем
        public Schedule Schedule { get; set; }
        public WeekDay WeekDay { get; set; } 
        public List<WeekDay> WeekDays { get; set; }  //дни недели для выбора пользователем
    }

    public class GroupViewModel
    {

    }
}
