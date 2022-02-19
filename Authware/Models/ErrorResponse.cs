using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Authware.Models
{
    /// <summary>
    ///     Represents an exception thrown by the Authware API
    /// </summary>
    public class ErrorResponse : BaseResponse
    {
        /// <summary>
        /// If the error was 500 this is the trace for the error
        /// </summary>
        [JsonProperty("trace")]
        public string Trace { get; set; }
        /// <summary>
        ///     A list of errors that were thrown by the Authware API, these are commonly data validation errors
        /// </summary>
        [JsonProperty("errors")]
        public List<string>? Errors { get; set; }
        
        /// <summary>
        /// Converts the ErrorResponse to a friendly format for display
        /// </summary>
        /// <returns>The formatted error response</returns>
        public override string ToString()
        {
            var errors = Errors?.Aggregate(string.Empty, (current, item) => current + $"{item}, ")
                .TrimEnd(' ')
                .TrimEnd(',');

            return $"{base.ToString()} {(errors is null ? string.Empty : $"({errors})")}";
        }
    }
}