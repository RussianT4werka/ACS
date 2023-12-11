using System;
using System.Collections.Generic;

namespace TelegramBot.BD;

public partial class Point
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
