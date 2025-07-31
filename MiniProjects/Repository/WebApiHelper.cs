namespace MiniProjects.Repository
{
    public class WebApiHelper
    {
        public class ApiResponseObj
        {
            public bool Success { get; set; }
            public string? message { get; set; }
            public object? data { get; set; }
            public string? transactionId { get; set; }
            public bool status { get; set; }

            public static ApiResponseObj Ok(object? data, string? message = null) => new() { Success = true, data = data, message = message };
            public static ApiResponseObj Fail(string message) => new() { Success = false, message = message };
        }
    }
}
