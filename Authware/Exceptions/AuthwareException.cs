using System;
using Authware.Models;

namespace Authware.Exceptions
{
    /// <summary>
    ///     Thrown when an exception relating to the response of the Authware API was received.
    /// </summary>
    public class AuthwareException : Exception
    {
        /// <summary>
        ///     The deserialized response from the API, this contains the error information.
        /// </summary>
        public readonly ErrorResponse? ErrorResponse;
        /// <summary>
        /// Constructs a new instance of an authware exception
        /// </summary>
        /// <param name="errorResponse"></param>
        public AuthwareException(ErrorResponse? errorResponse) : base(errorResponse?.Message)
        {
            ErrorResponse = errorResponse;
        }
    }
}