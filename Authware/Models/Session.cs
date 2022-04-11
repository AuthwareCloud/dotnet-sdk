using System;
using Newtonsoft.Json;

namespace Authware.Models;

/// <summary>
///     Represents an active session that the user possesses
/// </summary>
public class Session
{
    /// <summary>
    ///     The ID of the session
    /// </summary>
    [JsonProperty("id")]
    public Guid Id { get; set; }

    /// <summary>
    ///     The date the session was created
    /// </summary>

    [JsonProperty("date_created")]
    public DateTime DateCreated { get; set; }

    public override string ToString()
    {
        return Id.ToString();
    }
}