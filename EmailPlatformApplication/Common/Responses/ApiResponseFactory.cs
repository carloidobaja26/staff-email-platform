namespace EmailPlatform.Application.Common.Responses;

public static class ApiResponseFactory
{
    public static ApiResponse<T> Success<T>(
        T data,
        string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Validation<T>(
        string field,
        string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = "Validation failed.",
            Errors = new Dictionary<string, List<string>>
            {
                {
                    field,
                    new List<string>
                    {
                        message
                    }
                }
            }
        };
    }

    public static ApiResponse<T> NotFound<T>(
        string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message
        };
    }
}