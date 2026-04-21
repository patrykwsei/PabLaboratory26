using AppCore.Models.Common;

namespace AppCore.Models.Contacts;

public class Note : EntityBase
{
    public string Content { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid ContactId { get; set; }
    public Contact? Contact { get; set; }
}