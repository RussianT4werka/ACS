using System;
using System.Collections.Generic;

namespace ACS_API.DB;

public partial class Translate
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Event> EventDirNameNavigations { get; set; } = new List<Event>();

    public virtual ICollection<Event> EventPassDenies { get; set; } = new List<Event>();
}
