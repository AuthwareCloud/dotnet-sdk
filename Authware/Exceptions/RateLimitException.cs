using System;
using Authware.Models;

namespace Authware.Exceptions;

/// <summary>
///     Throws when you have been ratelimited
/// </summary>
public class RateLimitException : AuthwareException
{
    /// <summary>
    ///     This is how long you should wait before trying the request again
    /// </summary>
    public readonly TimeSpan RetryAfter;

    /// <summary>
    ///     Constructs a new instance of the rate limit exception
    /// </summary>
    /// <param name="errorResponse">An optional response</param>
    /// <param name="retryAfter">This is when you should retry</param>
    public RateLimitException(ErrorResponse? errorResponse, TimeSpan retryAfter) : base(errorResponse)
    {
        RetryAfter = retryAfter;
    }
}