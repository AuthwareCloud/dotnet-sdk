using Newtonsoft.Json;

namespace Authware.Models
{
    /// <summary>
    ///     The base response, all responses inherit from this class
    /// </summary>
    public class BaseResponse
    {
        /// <summary>
        ///     The HTTP status code of the response
        /// </summary>
        [JsonProperty("code")]
        public int Code { get; set; }

        /// <summary>
        ///     The message from the Authware API
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        ///     Auto property that defines whether a request was successful based on the response code
        /// </summary>
        public bool Success => Code == 200;

        public override string ToString()
        {
            return $"{Message} ({Code})";
        }
    }
}