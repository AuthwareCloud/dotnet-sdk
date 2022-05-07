using System;
using Newtonsoft.Json;

namespace Authware.Models;

/// <summary>
///     Represents the currently authenticated users profile
/// </summary>
public class Profile
{
    /// <summary>
    ///     The username of the user
    /// </summary>
    [JsonProperty("username")]
    public string Username { get; set; }

    /// <summary>
    ///     The ID of the user
    /// </summary>
    [JsonProperty("id")]
    public Guid Id { get; set; }

    /// <summary>
    ///     The email of the user
    /// </summary>
    [JsonProperty("email")]
    public string Email { get; set; }

    /// <summary>
    ///     The date the user was created
    /// </summary>

    [JsonProperty("date_created")]
    public DateTime DateCreated { get; set; }

    /// <summary>
    ///     The date the users access will expire
    /// </summary>
    [JsonProperty("expiration")]
    public DateTime Expiration { get; set; }

    /// <summary>
    ///     A list of active sessions that the may user possesses
    /// </summary>

    [JsonProperty("sessions")]
    public Session[]? Sessions { get; set; }

    /// <summary>
    ///     A list of roles the user possesses
    /// </summary>

    [JsonProperty("role")]
    public Role? Role { get; set; }

    /// <summary>
    ///     A list of previous API requests that this user has performed
    /// </summary>
    [JsonProperty("requests")]
    public ApiRequest[]? ApiRequests { get; set; }

    /// <summary>
    ///     A list of variables that the user possesses
    /// </summary>
    [JsonProperty("user_variables")]
    public UserVariable[]? UserVariables { get; set; }

    public override string ToString()
    {
        return $"{Username} ({Id})";
    }
}