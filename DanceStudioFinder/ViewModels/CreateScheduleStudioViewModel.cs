using DanceStudioFinder.Models;
using System.ComponentModel.DataAnnotations;

namespace DanceStudioFinder.ViewModels
{
    public class CreateScheduleStudioViewModel
    {
        public Admin Admin { get; set; }
        public DanceStudio DanceStudio { get; set; }

        public List<Style> Styles { get; set; } = new List<Style>();
        public List<WeekDay> WeekDays { get; set; } = new List<WeekDay>();

        public List<GroupViewModel> Groups { get; set; }
    }

    public class GroupViewModel
    {
        [StringLength(150, ErrorMessage = "Название группы должно быть не более 150 символов в длину\"")]
        [Required(ErrorMessage = "Название группы - обязательное поле для ввода")]
        public string Name { get; set; }
        public int StyleId { get; set; }

        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }

        [StringLength(500, ErrorMessage = "Описание группы не более 500 символов в длину")]
        public string? Description { get; set; }
        public List<ScheduleViewModel> Schedule { get; set; } = new();
    }

    public class ScheduleViewModel
    {
        public int DayOfWeekId { get; set; }

        [Required(ErrorMessage = "Укажите время начала")]
        public TimeOnly BeginTime { get; set; }

        [Required(ErrorMessage = "Укажите время окончания")]
        public TimeOnly EndTime { get; set; }
    }
}
