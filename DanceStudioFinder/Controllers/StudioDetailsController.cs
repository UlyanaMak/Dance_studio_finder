using DanceStudioFinder.Data;
using DanceStudioFinder.ViewModels;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace DanceStudioFinder.Controllers
{
    public class StudioDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudioDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Вывод страницы с информацией о студии
        /// </summary>
        /// <param name="studioId"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(int studioId)
        {
            var studio = await _context.DanceStudios
                .Include(s => s.IdAddressNavigation)
                .Include(s => s.Prices)
                .Include(s => s.DanceGroups)
                    .ThenInclude(g => g.IdStyleNavigation)
                .Include(s => s.DanceGroups)
                    .ThenInclude(g => g.IdAgeLimitNavigation)
                .Include(s => s.DanceGroups)
                    .ThenInclude(g => g.Schedules)
                        .ThenInclude(sch => sch.IdDayNavigation)
                .FirstOrDefaultAsync(s => s.IdStudio == studioId);

            if (studio == null)
            {
                return NotFound();
            }

            var viewModel = new StudioDetailsViewModel
            {
                Studio = studio,
                Groups = studio.DanceGroups.Select(g => new DanceGroupViewModel
                {
                    IdGroup = g.IdGroup,
                    Name = g.Name,
                    Description = g.Description,
                    Style = g.IdStyleNavigation,
                    AgeLimit = g.IdAgeLimitNavigation,
                    Schedule = g.Schedules.Select(s => new ScheduleDisplayModel
                    {
                        Day = s.IdDayNavigation,
                        BeginTime = s.BeginTime,
                        EndTime = s.EndTime
                    }).ToList()
                }).ToList(),
                Prices = studio.Prices.ToList(),
                WeekDays = _context.WeekDays.ToList(),  //выгрузка дней недели из БД
                Styles = _context.Styles.OrderBy(s => s.IdStyle).ToList()  //выгрузка стилей из БД
            };

            return View(viewModel);
        }

        public IActionResult ExportToWord(int id)
        {
            // Получение студии и связанных данных из БД
            var studio = _context.DanceStudios.FirstOrDefault(s => s.IdStudio == id);
            if (studio == null) return NotFound();

            var groups = _context.DanceGroups
                .Where(g => g.IdStudio == id)
                .Select(g => new DanceGroupViewModel
                {
                    IdGroup = g.IdGroup,
                    Name = g.Name,
                    Description = g.Description,
                    Style = g.IdStyleNavigation,
                    AgeLimit = g.IdAgeLimitNavigation,
                    Schedule = g.Schedules.Select(s => new ScheduleDisplayModel
                    {
                        IdSchedule = s.IdSchedule,
                        BeginTime = s.BeginTime,
                        EndTime = s.EndTime,
                        Day = s.IdDayNavigation
                    }).ToList()
                }).ToList();

            var weekDays = _context.WeekDays.ToList();

            var model = new StudioDetailsViewModel
            {
                Studio = studio,
                Groups = groups,
                WeekDays = weekDays
            };

            // Теперь вызываем экспорт по уже собранной модели
            return GenerateWordDocument(model);
        }


        public IActionResult GenerateWordDocument(StudioDetailsViewModel model)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (WordprocessingDocument doc = WordprocessingDocument.Create(stream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = doc.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = new Body();

                    // Название студии и дата
                    body.Append(new Paragraph(new Run(new Text($"Студия: {model.Studio.Name}"))));
                    body.Append(new Paragraph(new Run(new Text($"Дата экспорта: {DateTime.Now.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture)}"))));
                    body.Append(new Paragraph(new Run(new Text("")))); // пустая строка

                    // Получение всех временных слотов
                    var allTimeSlots = model.Groups
                        .SelectMany(g => g.Schedule)
                        .Select(s => new { s.BeginTime, s.EndTime })
                        .Distinct()
                        .OrderBy(t => t.BeginTime)
                        .ToList();

                    // Таблица
                    Table table = new Table();
                    table.AppendChild(new TableProperties(
                        new TableBorders(
                            new TopBorder { Val = BorderValues.Single, Size = 4 },
                            new BottomBorder { Val = BorderValues.Single, Size = 4 },
                            new LeftBorder { Val = BorderValues.Single, Size = 4 },
                            new RightBorder { Val = BorderValues.Single, Size = 4 },
                            new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                            new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
                        )));

                    // Заголовок таблицы
                    TableRow headerRow = new TableRow();
                    headerRow.Append(CreateCell("Время"));
                    foreach (var day in model.WeekDays)
                    {
                        headerRow.Append(CreateCell(day.Name));
                    }
                    table.Append(headerRow);

                    // Табличные строки по времени
                    foreach (var timeSlot in allTimeSlots)
                    {
                        TableRow row = new TableRow();
                        row.Append(CreateCell($"{timeSlot.BeginTime:HH\\:mm} – {timeSlot.EndTime:HH\\:mm}"));

                        foreach (var day in model.WeekDays)
                        {
                            var groupsInSlot = model.Groups
                                .Where(g => g.Schedule.Any(s =>
                                    s.Day.IdDay == day.IdDay &&
                                    s.BeginTime == timeSlot.BeginTime &&
                                    s.EndTime == timeSlot.EndTime))
                                .ToList();

                            var cell = new TableCell();

                            if (groupsInSlot.Any())
                            {
                                foreach (var group in groupsInSlot)
                                {
                                    var styleText = group.Style.NameEng != null && group.Style.NameRus != null
                                        ? $"{group.Style.NameEng} ({group.Style.NameRus})"
                                        : group.Style.NameEng ?? group.Style.NameRus;

                                    // Название группы — жирным
                                    cell.Append(new Paragraph(
                                        new Run(new RunProperties(new Bold()), new Text(group.Name))
                                    ));

                                    // Стиль
                                    cell.Append(new Paragraph(new Run(new Text(styleText))));

                                    // Возраст
                                    cell.Append(new Paragraph(new Run(new Text(group.AgeLimit.Name))));

                                    // Пустая строка между группами
                                    cell.Append(new Paragraph(new Run(new Text(""))));
                                }
                            }
                            else
                            {
                                // Пустая ячейка
                                cell.Append(new Paragraph(new Run(new Text(""))));
                            }

                            row.Append(cell);
                        }


                        table.Append(row);
                    }

                    body.Append(table);
                    // Альбомная ориентация страницы
                    var sectionProps = new SectionProperties(
                        new PageSize
                        {
                            Width = 16838,  // 29.7 см
                            Height = 11906, // 21 см
                            Orient = PageOrientationValues.Landscape
                        },
                        new PageMargin
                        {
                            Top = 1440,     // 2.54 см
                            Right = 1440,
                            Bottom = 1440,
                            Left = 1440,
                            Header = 720,
                            Footer = 720,
                            Gutter = 0
                        }
                    );

                    body.Append(sectionProps);

                    mainPart.Document.Append(body);
                }

                stream.Position = 0;
                string fileName = $"Расписание_{model.Studio.Name}_{DateTime.Now:dd.MM.yyyy}.docx";
                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    fileName);
            }
        }
        private TableCell CreateCell(string text)
        {
            return new TableCell(
                new Paragraph(
                    new Run(
                        new Text(text) { Space = SpaceProcessingModeValues.Preserve }
                    )
                )
            );
        }
    }
}
