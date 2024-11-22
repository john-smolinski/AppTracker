namespace ApplicationTracker.Common
{
#pragma warning disable CS8618
    public class ErrorResponse
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public string Detail {  get; set; }
    }
#pragma warning restore CS8618
}
