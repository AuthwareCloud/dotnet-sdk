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
        public ResponseStatus Code { get; set; }

        /// <summary>
        ///     The message from the Authware API
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        ///     Auto property that defines whether a request was successful based on the response code
        /// </summary>
        public bool Success => Code == ResponseStatus.Success;

        public override string ToString()
        {
            return $"{Message} ({Code})";
        }
        
        public enum ResponseStatus
        {
            Success = 0,
            SessionExpired = 1,
            ServerNotFound = 2,
            AuthenticationFailed = 3,
            SessionNotFound = 4,
            AppNotFound = 5,
            UserNotFound = 6,
            VariableNotFound = 7,
            ApiNotFound = 8,
            RoleNotFound = 9,
            TokenNotFound = 10,
            Recaptcha = 11,
            AlreadyAuthenticated = 12,
            MissingRolePermissions = 13,
            ValidationError = 14,
            InternalServerError = 15,
            ApiExecutionFailed = 16,
            ApplicationLimits = 17,
            MissingUserVariablePermissions = 18,
            Fraud = 19,
            InvalidHardwareId = 20,
            UpdateRequired = 21,
            MissingVersionHeader = 22,
            IdentifierMissing = 23,
            Unidentified
        }
    }
}