using System;
using Newtonsoft.Json;

namespace Authware.Models;

/// <summary>
///     Represents a variable from either an application or a role
/// </summary>
public class Variable
{
    /// <summary>
    ///     The ID of the variable
    /// </summary>
    [JsonProperty("id")]
    public Guid Id { get; set; }

    /// <summary>
    ///     The key of the variable
    /// </summary>
    [JsonProperty("key")]
    public string Key { get; set; }

    /// <summary>
    ///     The value of the variable
    /// </summary>
    [JsonProperty("value")]
    public string Value { get; set; }

    public void Deconstruct(out string key, out string value)
    {
        key = Key;
        value = Value;
    }

    public override string ToString()
    {
        return $"{Key}: {Value}";
    }
}