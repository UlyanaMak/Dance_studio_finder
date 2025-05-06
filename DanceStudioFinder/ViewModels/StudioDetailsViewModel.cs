using DanceStudioFinder.Models;

namespace DanceStudioFinder.ViewModels
{
    public class StudioDetailsViewModel
    {
        public DanceStudio Studio { get; set; } = null!;

        //группы в студии
        public List<DanceGroupViewModel> Groups { get; set; } = new();

        public List<Price> Prices { get; set; } = new();

        public List<Style> Styles { get; set; } = new();
        public List<WeekDay> WeekDays { get; set; } = new();
    }
    //класс для группы
    public class DanceGroupViewModel
    {
        public int IdGroup { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public Style Style { get; set; } = null!;
        public AgeLimit AgeLimit { get; set; } = null!;
        //расписание группы
        public List<ScheduleDisplayModel> Schedule { get; set; } = new();
    }
    //класс расписания
    public class ScheduleDisplayModel
    {
        public int IdSchedule { get; set; }
        public WeekDay Day { get; set; } = null!;
        public TimeOnly BeginTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
