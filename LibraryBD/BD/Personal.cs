using System;
using System.Collections.Generic;

namespace LibraryBD.BD;

public partial class Personal
{
    public int Id { get; set; }

    public string Fio { get; set; } = null!;

    public string? Department { get; set; }

    public string? Position { get; set; }

    public byte[]? Image { get; set; }

    public string? Dec { get; set; }

    public string? W26 { get; set; }

    public string? Hex { get; set; }

    public int? AdminId { get; set; }

    public virtual Admin? Admin { get; set; }
}
