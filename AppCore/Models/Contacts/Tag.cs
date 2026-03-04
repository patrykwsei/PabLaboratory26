using AppCore.Models.Common;

namespace AppCore.Models.Contacts;

public class Tag : EntityBase
{
    public string Name { get; set; } = "";
    public string Color { get; set; } = "";
}