using System;
using System.Collections.Generic;

namespace ACS_API.DB;

public partial class Point
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
