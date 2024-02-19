using System;
using System.Collections.Generic;

namespace LibraryBD.BD;

public partial class Admin
{
    public int Id { get; set; }

    public string? Surname { get; set; }

    public string? Name { get; set; }

    public string? Patronymic { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();
}
