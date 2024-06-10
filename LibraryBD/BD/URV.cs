using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryBD.BD
{
    public partial class URV
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string FIO { get; set; }
        public string Position { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan TotalTime { get; set; }

        public DateTime StartTime2 { get; set; }
        public DateTime EndTime2 { get; set; }
    }
}
