using System;
using Newtonsoft.Json;

namespace Authware.Models;

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
    public string? Message { get; set; }

    /// <summary>
    ///     Auto property that defines whether a request was successful based on the response code
    /// </summary>
    public bool Success => Code == ResponseStatus.Success;

    /// <summary>
    /// Formats the response to a proper string you can print out
    /// </summary>
    /// <returns>The formatted string</returns>
    public override string ToString()
    {
        return $"{Message} ({Code})";
    }

    /// <summary>
    ///     Contains every error code that can be thrown by the API as an enum
    /// </summary>
    public enum ResponseStatus : ushort
    {
        /// <summary>
        ///     No error was encountered
        /// </summary>
        Success = 0,

        /// <summary>
        ///     The session associated with the authentication token has expired and needs renewing
        /// </summary>
        SessionExpired = 1,

        /// <summary>
        ///     The OpenVPN server specified in a request wasn't found
        /// </summary>
        ServerNotFound = 2,

        /// <summary>
        ///     User authentication failed due to an invalid username, password or other factors
        /// </summary>
        AuthenticationFailed = 3,

        /// <summary>
        ///     The session specified in a request wasn't found
        /// </summary>
        SessionNotFound = 4,

        /// <summary>
        ///     The app specified in a request wasn't found
        /// </summary>
        AppNotFound = 5,

        /// <summary>
        ///     The user specified in a request wasn't found
        /// </summary>
        UserNotFound = 6,

        /// <summary>
        ///     The variable specified in a request wasn't found
        /// </summary>
        VariableNotFound = 7,

        /// <summary>
        ///     The API specified in a request wasn't found
        /// </summary>
        ApiNotFound = 8,

        /// <summary>
        ///     The role specified in a request wasn't found
        /// </summary>
        RoleNotFound = 9,

        /// <summary>
        ///     The token specified in a request wasn't found, this can be thrown by RegisterAsync
        /// </summary>
        TokenNotFound = 10,

        /// <summary>
        ///     The reCAPTCHA token was invalid, this is not thrown by app endpoints and should not be used
        /// </summary>
        [Obsolete("This is not thrown by app endpoints and should not be used", true)]
        Recaptcha = 11,

        /// <summary>
        ///     The request cannot be completed as you're already authenticated
        /// </summary>
        AlreadyAuthenticated = 12,

        /// <summary>
        ///     Permissions to perform the request are missing via a role, this is thrown by ExecuteApiAsync
        /// </summary>
        MissingRolePermissions = 13,

        /// <summary>
        ///     The data submitted is not acceptable by Authware, the validation errors are present in the Errors array
        /// </summary>
        ValidationError = 14,

        /// <summary>
        ///     The Authware API encountered an exception whilst processing the request, you should report this to us
        /// </summary>
        InternalServerError = 15,

        /// <summary>
        ///     API execution has failed, as the API in question returned a non-OK (200) HTTP response code
        /// </summary>
        ApiExecutionFailed = 16,

        /// <summary>
        ///     The application has hit it's limits for the plan it possesses, you need to upgrade your account to continue
        /// </summary>
        ApplicationLimits = 17,

        /// <summary>
        ///     An attempt to modify or create user variables failed, as the user does not possess the permissions to modify the
        ///     variable
        /// </summary>
        MissingUserVariablePermissions = 18,

        /// <summary>
        ///     The fraud detection system has flagged the request and as a result, the request cannot be serviced. This is not
        ///     thrown by app endpoints and should not be used
        /// </summary>
        [Obsolete("This is not thrown by app endpoints and should not be used", true)]
        Fraud = 19,

        /// <summary>
        ///     The hardware ID did not match the hardware ID associated with the requested user
        /// </summary>
        InvalidHardwareId = 20,

        /// <summary>
        ///     An application update was detected, and the application needs to be updated to continue
        /// </summary>
        UpdateRequired = 21,

        /// <summary>
        ///     The application version is missing from the HTTP request headers, this being thrown is a SDK issue and should be
        ///     reported
        /// </summary>
        MissingVersionHeader = 22,

        /// <summary>
        ///     The hardware ID is missing from the HTTP request headers, this being thrown is a SDK issue and should be reported
        /// </summary>
        IdentifierMissing = 23,

        /// <summary>
        ///     The request could not be serviced as too many request from your IP have been detected, you need to wait a minute
        ///     before trying again
        /// </summary>
        Ratelimited = 24,

        /// <summary>
        ///     The error thrown is not able to be identified by this error scheme
        /// </summary>
        Unidentified = ushort.MaxValue
    }
}