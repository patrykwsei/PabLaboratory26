namespace AppCore.Dto;

public record NoteDto
{
    public Guid Id { get; init; }
    public string Content { get; init; } = "";
    public DateTime CreatedAt { get; init; }
}