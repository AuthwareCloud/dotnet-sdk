using System;
using Newtonsoft.Json;

namespace Authware.Models;

/// <summary>
///     Represents the data returned from an application data request
/// </summary>
public class Application
{
    /// <summary>
    ///     The friendly name of your application
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    ///     The ID of your application
    /// </summary>
    [JsonProperty("id")]
    public Guid Id { get; set; }

    /// <summary>
    ///     The set version of your application
    /// </summary>
    [JsonProperty("version")]
    public Version Version { get; set; }

    /// <summary>
    ///     The date your application was created
    /// </summary>
    [JsonProperty("date_created")]
    public DateTime Created { get; set; }
    
    /// <summary>
    /// Tells you if your application has hwid locking enabled
    /// </summary>
    [JsonProperty("is_hwid_checking_enabled")]
    public bool CheckIdentifier { get; set; }
    /// <summary>
    ///     A list of APIs that is implemented in your application
    /// </summary>
    [JsonProperty("apis")]
    public Api[] Apis { get; set; }

    public override string ToString()
    {
        return $"{Name} (v{Version})";
    }
}