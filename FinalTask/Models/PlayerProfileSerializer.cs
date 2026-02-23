using System.Text.Json;

namespace FinalTask.Models;

public static class PlayerProfileSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = false
    };

    public static string Serialize(PlayerProfile profile)
    {
        return JsonSerializer.Serialize(profile, Options);
    }

    public static PlayerProfile? Deserialize(string json)
    {
        return JsonSerializer.Deserialize<PlayerProfile>(json, Options);
    }
}