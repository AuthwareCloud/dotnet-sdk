namespace Authware.Models
{
    /// <summary>
    ///     A wrapper around a instanced Authware request
    /// </summary>
    /// <typeparam name="T">The type of response that is being wrapped around</typeparam>
    public class StaticResponse<T> : ErrorResponse
    {
        /// <summary>
        ///     The response received by the Authware API, this is the data that is returned on a successful response
        /// </summary>
        public T Response { get; set; }
    }
}