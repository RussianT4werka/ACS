using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryBD.BD;
using Microsoft.Extensions.Logging;
using Spire.Xls;
using System.Data;
using Microsoft.Extensions.FileProviders;
using Elfie.Serialization;
using Telegram.Bot.Types;

namespace ACS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class URVController : ControllerBase
    {
        private readonly AcsContext _context;
        private List<URV> ListURV;
        private DateTime DateStart;
        private DateTime DateEnd;

        public URVController(AcsContext context)
        {
            _context = context;
        }

        [HttpGet("GetListURV")]
        public async Task<ActionResult<List<URV>>> GetListURV(string dateStart, string dateEnd)
        {
            ListURV = new();
            DateStart = Convert.ToDateTime(dateStart);
            DateEnd = Convert.ToDateTime(dateEnd);
            try
            {
                if (_context.Events == null)
                {
                    return Problem("Entity set 'AcsContext.Events'  is null.");
                }
                else
                {
                    var listPersoanl = _context.Personals.Where(s => s.Position != "Водитель АБС").ToList();
                    if (DateStart == DateEnd)
                    {
                        foreach (var person in listPersoanl)
                        {
                            var startTimePerson = _context.Events.ToList().FirstOrDefault(s => s.Fio == person.Fio && Convert.ToDateTime(s.Time).Date == DateStart.Date);
                            var endTimePerson = _context.Events.ToList().LastOrDefault(s => s.Fio == person.Fio && Convert.ToDateTime(s.Time).Date == DateStart.Date);
                            if (startTimePerson != null && endTimePerson != null)
                            {
                                DateTime startTime = Convert.ToDateTime(startTimePerson.Time);
                                DateTime endTime = Convert.ToDateTime(endTimePerson.Time);

                                TimeSpan start = startTime.TimeOfDay;
                                TimeSpan end = endTime.TimeOfDay;

                                TimeSpan totalTime = end - start;

                                var urv = new URV() { Date = Convert.ToDateTime(DateStart), FIO = startTimePerson.Fio, Position = person.Position, StartTime = startTime.TimeOfDay, EndTime = endTime.TimeOfDay, TotalTime = totalTime, StartTime2 = Convert.ToDateTime(dateStart), EndTime2 = Convert.ToDateTime(endTime) };
                                ListURV.Add(urv);
                            }
                        }
                    }
                    else
                    {
                        
                        while (DateStart <= DateEnd)
                        {
                            foreach (var person in listPersoanl)
                            {
                                var startTimePerson = _context.Events.ToList().FirstOrDefault(s => s.Fio == person.Fio && Convert.ToDateTime(s.Time).Date == DateStart.Date);
                                var endTimePerson = _context.Events.ToList().LastOrDefault(s => s.Fio == person.Fio && Convert.ToDateTime(s.Time).Date == DateStart.Date);
                                if (startTimePerson != null && endTimePerson != null)
                                {
                                    DateTime startTime = Convert.ToDateTime(startTimePerson.Time);
                                    DateTime endTime = Convert.ToDateTime(endTimePerson.Time);

                                    TimeSpan start = startTime.TimeOfDay;
                                    TimeSpan end = endTime.TimeOfDay;

                                    TimeSpan totalTime = end - start;

                                    var urv = new URV() { Date = Convert.ToDateTime(DateStart), FIO = startTimePerson.Fio, Position = person.Position, StartTime = startTime.TimeOfDay, EndTime = endTime.TimeOfDay, TotalTime = totalTime, StartTime2 = Convert.ToDateTime(dateStart), EndTime2 = Convert.ToDateTime(endTime) };
                                    ListURV.Add(urv);
                                }
                            }
                            DateStart += new TimeSpan(1, 0, 0, 0);
                        }
                    }
                    return Ok(ListURV);
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CreateReportExcel")]
        public async Task<ActionResult> CreateReportExcel(List<URV> listURV)
        {
            try
            {
                var randomUrvForPeriod = listURV.FirstOrDefault();

                DateTime Time = DateTime.Now;
                //Создание рабочего пространства
                Workbook wb = new Workbook();

                //Удаление стандартного листа
                wb.Worksheets.Clear();

                //Добавление кастомного листа "Лист 1"
                Worksheet sheet = wb.Worksheets.Add("Лист 1");

                //Слияние ячекк от А1 до F1
                sheet.Range["A1:F1"].Merge();

                //Заголовок таблицы в A1
                sheet.Range["A1"].Value = $"Табель трудовой дисциплины на момент {Time}\nС: {randomUrvForPeriod.StartTime2.ToString("dd.MM.yyyy")} По: {randomUrvForPeriod.EndTime2.ToString("dd.MM.yyyy")}";
                sheet.Range["A1"].HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range["A1"].VerticalAlignment = VerticalAlignType.Center;
                sheet.Range["A1"].Style.Font.IsBold = true;
                sheet.Range["A1"].Style.Font.Size = 13F;
                //Высота первой строки
                sheet.Rows[0].RowHeight = 30F;

                //Создание таблицы
                DataTable dt = new DataTable();
                // Строка с заголовками
                dt.Columns.Add("Дата");

                dt.Columns.Add("ФИО");

                dt.Columns.Add("Должность");

                dt.Columns.Add("Начала\nфактических\nприсутствий");

                dt.Columns.Add("Окончания\nфактических\nприсутствий");

                dt.Columns.Add("Фактическая\nнаработка");

                // Создание строк
                foreach (var urv in listURV)
                {
                    dt.Rows.Add(urv.Date.ToString("dd-MM-yyyy"), urv.FIO, urv.Position, urv.StartTime, urv.EndTime, urv.TotalTime);
                }

                //Import data from DataTable to worksheet
                sheet.InsertDataTable(dt, true, 2, 1, true);

                //Настройка колонок/ячеек
                sheet.Range["A2:F7"].RowHeight = 15F;
                
                sheet.Range["A2:F7"].Columns[2].ColumnWidth = 50F;

                sheet.Range["A2"].ColumnWidth = 20F;
                sheet.Range["A2"].HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range["A2"].VerticalAlignment = VerticalAlignType.Center;
                sheet.Range["A2"].Style.Font.IsBold = true;
                sheet.Range["A2"].Style.Font.Size = 10F;

                sheet.Range["B2"].ColumnWidth = 35F;
                sheet.Range["B2"].HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range["B2"].VerticalAlignment = VerticalAlignType.Center;
                sheet.Range["B2"].Style.Font.IsBold = true;
                sheet.Range["B2"].Style.Font.Size = 10F;

                sheet.Range["C2"].ColumnWidth = 20F;
                sheet.Range["C2"].HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range["C2"].VerticalAlignment = VerticalAlignType.Center;
                sheet.Range["C2"].Style.Font.IsBold = true;
                sheet.Range["C2"].Style.Font.Size = 10F;

                sheet.Range["D2"].ColumnWidth = 20F;
                sheet.Range["D2"].RowHeight = 40F;
                sheet.Range["D2"].HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range["D2"].VerticalAlignment = VerticalAlignType.Center;
                sheet.Range["D2"].Style.Font.IsBold = true;
                sheet.Range["D2"].Style.Font.Size = 10F;

                sheet.Range["E2"].ColumnWidth = 20F;
                sheet.Range["E2"].RowHeight = 40F;
                sheet.Range["E2"].HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range["E2"].VerticalAlignment = VerticalAlignType.Center;
                sheet.Range["E2"].Style.Font.IsBold = true;
                sheet.Range["E2"].Style.Font.Size = 10F;

                sheet.Range["F2"].ColumnWidth = 20F;
                sheet.Range["F2"].RowHeight = 40F;
                sheet.Range["F2"].HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range["F2"].VerticalAlignment = VerticalAlignType.Center;
                sheet.Range["F2"].Style.Font.IsBold = true;
                sheet.Range["F2"].Style.Font.Size = 10F;

                //Рамки
                sheet.Range[$"A2:F{listURV.Count() + 2}"].BorderAround(LineStyleType.Medium); // жирная рамка вокруг
                sheet.Range[$"A2:F{listURV.Count() + 2}"].BorderInside(LineStyleType.Thin); // линии внутри
                sheet.Range["A2:F2"].BorderAround(LineStyleType.Medium); // рамка вокруг наименований колонок
                sheet.Range["A2:F7"].Borders.KnownColor = ExcelColors.Black;

                //Сохранение отчёта в формате .xlsx 
                string fileName = $"Табель трудовой дисциплины с {DateStart} по {DateEnd}";
                wb.SaveToFile($"C:/Users/kokorin.av/source/repos/ACS/ACS_BlazorView/wwwroot/Учёты рабочего времени/Учёт рабочего времени.xlsx", FileFormat.Version2016);

                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
