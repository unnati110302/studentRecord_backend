public class OpenIdConnectMetadata
{
    public string Issuer { get; set; }
    public IEnumerable<KeyInfo> Keys { get; set; }
}

public class KeyInfo
{
    public IEnumerable<string> X5c { get; set; }
}
