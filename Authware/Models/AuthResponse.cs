using Newtonsoft.Json;

namespace Authware.Models;

/// <summary>
///     Represents the authentication token returned by a successful authentication request
/// </summary>
public class AuthResponse
{
    /// <summary>
    ///     The Authware authentication token returned from the API, this can be used to authorize Authware API requests for
    ///     your application
    /// </summary>
    [JsonProperty("auth_token")]
    public string AuthToken { get; set; }

    public override string ToString()
    {
        return AuthToken;
    }
}