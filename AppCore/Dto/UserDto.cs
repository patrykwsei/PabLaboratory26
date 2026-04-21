namespace AppCore.Dto;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName =>
        $"{FirstName} {LastName}".Trim();

    public string Department { get; set; } = string.Empty;

    public string? Position { get; set; }

    public int Status { get; set; }

    public IEnumerable<string> Roles { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}