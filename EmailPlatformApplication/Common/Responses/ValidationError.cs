namespace EmailPlatform.Application.Common.Responses;

public class ValidationError
{
    public string Field { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}