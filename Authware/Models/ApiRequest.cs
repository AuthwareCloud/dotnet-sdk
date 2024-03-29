﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Authware.Models;

/// <summary>
///     Represents a previous request made by a user for a specific API
/// </summary>
public class ApiRequest
{
    /// <summary>
    ///     The ID of the request, this is generated by the API and can be seen in your applications audit logs
    /// </summary>
    [JsonProperty("id")]
    public Guid Id { get; set; }

    /// <summary>
    ///     The ID of the API that was executed
    /// </summary>
    [JsonProperty("api_id")]
    public Guid ApiId { get; set; }

    /// <summary>
    ///     The parameters the user sent, this does not include parameters that already have a value set in your application
    /// </summary>
    [JsonProperty("parameters")]
    public Dictionary<string, string>? Parameters { get; set; }

    /// <summary>
    ///     The date the request was sent
    /// </summary>
    [JsonProperty("date_created")]
    public DateTime DateCreated { get; set; }
}