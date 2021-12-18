using System;
using Newtonsoft.Json;

namespace Authware.Models
{
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

        public override string ToString()
        {
            return $"{Name} (v{Version})";
        }
    }
}