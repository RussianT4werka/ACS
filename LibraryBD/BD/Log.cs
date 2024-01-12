using System;
using System.Collections.Generic;

namespace LibraryBD.BD;

public partial class Log
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public DateTime DateTime { get; set; }
}
