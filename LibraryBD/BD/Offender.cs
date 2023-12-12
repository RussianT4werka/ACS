using System;
using System.Collections.Generic;

namespace LibraryBD.BD;

public partial class Offender
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Position { get; set; }

    public string? Dec { get; set; }

    public string? W26 { get; set; }

    public string? Hex { get; set; }

    public DateTime? Time { get; set; }

    public byte? SendOrNot { get; set; }
}
