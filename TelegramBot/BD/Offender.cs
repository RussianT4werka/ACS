using System;
using System.Collections.Generic;

namespace TelegramBot.BD;

public partial class Offender
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Position { get; set; }

    public string? Dec { get; set; }

    public string? W26 { get; set; }

    public string? Hex { get; set; }
}
