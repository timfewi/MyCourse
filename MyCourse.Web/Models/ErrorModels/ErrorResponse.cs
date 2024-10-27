namespace MyCourse.Web.Models.ErrorModels
{
    public class ErrorResponse
    {
        public string Error { get; set; } = string.Empty;
        public string? Code { get; set; }
        public object? Details { get; set; }
    }
}
