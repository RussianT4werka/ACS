using System;
using System.Collections.Generic;

namespace LibraryBD.BD;

public partial class Admin
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Surname { get; set; }

    public string? Name { get; set; }

    public string? Patronymic { get; set; }

    public virtual ICollection<Personal> Personals { get; set; } = new List<Personal>();
}
