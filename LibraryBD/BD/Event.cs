using System;
using System.Collections.Generic;

namespace LibraryBD.BD;

public partial class Event
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Position { get; set; }

    public string? Dec { get; set; }

    public string? W26 { get; set; }

    public string? Hex { get; set; }

    public string PassDenyId { get; set; } = null!;

    public string DirName { get; set; } = null!;

    public int PointId { get; set; }

    public string Time { get; set; } = null!;

    public DateTime? TimeConverted { get; set; }

    public virtual ICollection<Cycle> Cycles { get; set; } = new List<Cycle>();

    public virtual Translate DirNameNavigation { get; set; } = null!;

    public virtual Translate PassDeny { get; set; } = null!;

    public virtual Point Point { get; set; } = null!;
}
