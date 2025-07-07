namespace TastyGo.Helpers
{
    public class ServiceResponse<T>
    {
        public ResponseStatus Status { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public string StatusCode { get; set; }

        public ServiceResponse(ResponseStatus status, T data, string message, AppStatusCode statusCode)
        {
            Status = status;
            Data = data;
            Message = message;
            StatusCode = ((int)statusCode).ToString("D4");

        }

    }

}
