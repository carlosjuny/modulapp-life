namespace Infrastructure.Auth;

public class SocialAuthOptions
{
    public const string SectionName = "Social";

    public ProviderOptions Google { get; set; } = new();

    public ProviderOptions Apple { get; set; } = new();

    public class ProviderOptions
    {
        public string[] ClientIds { get; set; } = [];
    }
}
