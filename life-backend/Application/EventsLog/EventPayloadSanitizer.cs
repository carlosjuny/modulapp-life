using System.Text.Json;

namespace Application.EventsLog;

public static class EventPayloadSanitizer
{
    private static readonly string[] SensitiveFragments =
    [
        "password", "passwd", "contrasena", "token", "secret", "apikey", "api_key", "authorization", "credential"
    ];

    public static string ToSafeJson(IReadOnlyDictionary<string, string?>? payload)
    {
        var safe = (payload ?? new Dictionary<string, string?>())
            .Where(p => !IsSensitive(p.Key))
            .ToDictionary(p => p.Key, p => p.Value);

        return JsonSerializer.Serialize(safe);
    }

    private static bool IsSensitive(string key)
        => SensitiveFragments.Any(f => key.Contains(f, StringComparison.OrdinalIgnoreCase));
}
