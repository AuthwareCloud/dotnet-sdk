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
        ///     A list of errors that were thrown by the Authware API, these are commonly data validation errors
        /// </summary>
        [JsonProperty("errors")]
        public string[] Errors { get; set; }
        public override string ToString()
        {
            var errors = Errors?.Aggregate(string.Empty, (current, item) => current + $"{item}, ")
                .TrimEnd(' ')
                .TrimEnd(',');

            return $"{base.ToString()} {(errors is null ? string.Empty : $"({errors})")}";
        }
    }
}