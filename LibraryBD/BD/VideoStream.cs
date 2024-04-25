using System;
using System.Collections.Generic;

namespace LibraryBD.BD;

public partial class VideoStream
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Link { get; set; } = null!;

    public string? LinkOpenDoor { get; set; }
}
