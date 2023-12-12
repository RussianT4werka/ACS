using System;
using System.Collections.Generic;

namespace LibraryBD.BD;

public partial class SubscriberTelegramBot
{
    public int ChatId { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Username { get; set; }

    public byte? SubscribeOrNot { get; set; }
}
