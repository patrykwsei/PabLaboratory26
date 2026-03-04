using AppCore.Models.Contacts.Enums;

namespace AppCore.Models.Contacts;

public class Person : Contact
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string? MiddleName { get; set; }

    public DateTime? BirthDate { get; set; }

    public Gender Gender { get; set; }

    public string? Position { get; set; }

    public Organization? Organization { get; set; }
    public Company? Employer { get; set; }

    public override string GetDisplayName()
    {
        return $"{FirstName} {LastName}";
    }
}