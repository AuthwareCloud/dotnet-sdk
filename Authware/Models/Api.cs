using System;
using Newtonsoft.Json;

namespace Authware.Models;

/// <summary>
///     Represents an API that has been added to your application, this contains the name of your API and the ID, this can
///     be useful for listing your APIs in a list for the user to decide what to execute.
/// </summary>
public class Api
{
    /// <summary>
    ///     The ID of your API
    /// </summary>
    [JsonProperty("id")]
    public Guid Id { get; set; }

    /// <summary>
    ///     The friendly name of your API
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }
}