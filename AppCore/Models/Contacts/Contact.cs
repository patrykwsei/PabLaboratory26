using AppCore.Models.Common;
using AppCore.Models.Contacts.Enums;

namespace AppCore.Models.Contacts;

public abstract class Contact : EntityBase
{
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";

    public Address? Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ContactStatus Status { get; set; } = ContactStatus.Active;

    public List<Tag> Tags { get; set; } = new();
    public List<Note> Notes { get; set; } = new();

    public abstract string GetDisplayName();
}