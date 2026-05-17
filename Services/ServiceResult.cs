namespace CarparkManagementSystem.Services;

public class ServiceResult
{
    public bool Succeeded { get; init; }

    public string Message { get; init; } = string.Empty;

    public static ServiceResult Success(string message) => new() { Succeeded = true, Message = message };

    public static ServiceResult Failure(string message) => new() { Succeeded = false, Message = message };
}
