namespace MCR.API.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }

        public ApiResponse()
        {
            Success = true;
        }

        public static ApiResponse<T> Ok(T data, string message = null)
        {
            return new ApiResponse<T> { Success = true, Data = data, Message = message };
        }

        public static ApiResponse<T> Fail(string message, List<string> errors = null)
        {
            return new ApiResponse<T> { Success = false, Message = message, Errors = errors };
        }
    }

    public class PaginationRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SortBy { get; set; }
        public bool SortDesc { get; set; }
        public string Search { get; set; }
    }

    public class PaginationResponse<T>
    {
        public List<T> Items { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public PaginationResponse(List<T> items, int total, int page, int pageSize)
        {
            Items = items;
            TotalItems = total;
            CurrentPage = page;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(total / (double)pageSize);
        }
    }
}
