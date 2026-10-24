using System.Text.Json;
using Application.EventsLog;

namespace Application.Tests.EventsLog;

public class EventPayloadSanitizerTests
{
    [Fact]
    public void ToSafeJson_ConClaveSensible_LaExcluyeDelResultado()
    {
        var payload = new Dictionary<string, string?>
        {
            ["email"] = "ana@example.com",
            ["password"] = "secreto"
        };

        var json = EventPayloadSanitizer.ToSafeJson(payload);

        var resultado = JsonSerializer.Deserialize<Dictionary<string, string?>>(json)!;
        Assert.Single(resultado);
        Assert.Equal("ana@example.com", resultado["email"]);
    }
}
