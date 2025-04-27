using System.Text.Json.Serialization;

namespace RO.DevTest.Application.Features.Auth.Commands.RegisterCommand;

public record RegisterResponse {
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? UserId { get; set; } = null;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Username { get; set; } = null;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Email { get; set; } = null;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? Roles { get; set; } = null;
}
