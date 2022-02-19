using Newtonsoft.Json;

namespace Authware.Models
{
    /// <summary>
    ///     Represents a variable from either an application or a role
    /// </summary>
    public class Variable
    {
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

        /// <summary>
        /// This is mostly used for deconstructing types to a tuple
        /// </summary>
        /// <param name="key">The key of the variable</param>
        /// <param name="value">The value of the variable</param>
        public void Deconstruct(out string key, out string value)
        {
            key = Key;
            value = Value;
        }

        /// <summary>
        /// Gives you the variable in Key: Value format
        /// </summary>
        /// <returns>The variable formatted into {Key}: {Value}</returns>
        public override string ToString()
        {
            return $"{Key}: {Value}";
        }
    }
}