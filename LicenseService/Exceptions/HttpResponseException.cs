namespace LicenseService.Exceptions
{
    public class HttpResponseException : Exception
    {
        public HttpResponseException(int statusCode, string responseMessage) =>
        (StatusCode, ReponseMessage) = (statusCode, responseMessage);

        public int StatusCode { get; }
        public object ReponseMessage { get; }

    }
}
