using System;
using System.Collections.Generic;

namespace LibraryBD.BD;

public partial class Cycle
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public string W26 { get; set; } = null!;

    public DateTime? TimeP1 { get; set; }

    public DateTime? TimeP2 { get; set; }

    public TimeSpan? Delta { get; set; }

    public byte SendOrNot { get; set; }

    public virtual Event Event { get; set; } = null!;
}
