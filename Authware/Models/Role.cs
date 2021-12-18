using System;
using Newtonsoft.Json;

namespace Authware.Models
{
    /// <summary>
    ///     Represents a role that the authenticated user may posses
    /// </summary>
    public class Role
    {
        /// <summary>
        ///     The ID of the role
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        ///     The name of the role
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     The variables that the role possesses
        /// </summary>
        [JsonProperty("variables")]
        public Variable[] Variables { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
    }
}