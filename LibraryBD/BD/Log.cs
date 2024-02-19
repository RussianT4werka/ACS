using System;
using System.Collections.Generic;

namespace LibraryBD.BD;

public partial class Log
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public DateTime DateTime { get; set; }

    public int? AdminId { get; set; }

    public int? PersonalId { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Personal? Personal { get; set; }
}
