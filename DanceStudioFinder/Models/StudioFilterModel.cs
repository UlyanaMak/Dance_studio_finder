using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DanceStudioFinder.Models
{
    public class StudioFilterModel
    {
        [StringLength(255, ErrorMessage = "Название студии должно быть не более {1} символов в длину")]
        public string? Name { get; set; }

        [StringLength(30, ErrorMessage = "Название населённого пункта не более 30 символов")]
        [RegularExpression(@"^[А-Яа-яЁё\s\-]+$", ErrorMessage = "В названии населённого пункта разрешены только русские буквы")]
        public string? Locality { get; set; }

        [StringLength(30, ErrorMessage = "Название района не более 30 символов")]
        public string? SettlementArea { get; set; }

        [StringLength(135, ErrorMessage = "Название улицы не более 135 символов")]
        [RegularExpression(@"^[А-Яа-яЁё\s\-]+$", ErrorMessage = "В названии улицы разрешены только русские буквы")]
        public string? Street { get; set; }

        public string? Style { get; set; }
        public List<Style?> Styles { get; set; } = new List<Style>();

        public TimeOnly? BeginTime { get; set; }
        public TimeOnly? EndTime { get; set; }

        public int? MaxPrice { get; set; }

        public int? Age { get; set; }
    }
}
