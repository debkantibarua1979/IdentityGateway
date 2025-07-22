namespace IdentityService.Dtos;

public class GenericResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }

    public static GenericResult Ok(string? message = null) => new() { Success = true, Message = message };
    public static GenericResult Fail(string message) => new() { Success = false, Message = message };
}