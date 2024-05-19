using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LibraryBD.BD;

public partial class Event
{
    public int Id { get; set; }

    public int InternalId { get; set; }

    public string? Fio { get; set; }

    public string? Position { get; set; }

    public string? Dec { get; set; }

    public string? W26 { get; set; }

    public string? Hex { get; set; }

    public string? PassOrDeny { get; set; }

    public string? DirName { get; set; }

    public int? PointId { get; set; }

    public string? Time { get; set; }

    public byte SendOrNot { get; set; }
    [JsonIgnore]
    public virtual ICollection<Cycle> Cycles { get; set; } = new List<Cycle>();
}
