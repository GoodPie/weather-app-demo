namespace DAL.Dtos;

public class ServiceResponse<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; } = true;
    public string? Message { get; set; } = null;
    public string? Error { get; set; } = null;
    public List<string>? ErrorMessages { get; set; } = null;
    
    public ServiceResponse(T data)
    {
        Data = data;
    }
    
    public ServiceResponse(string message)
    {
        Message = message;
        Success = false;
    }
    
    public ServiceResponse(string error, List<string>? errorMessages = null)
    {
        Error = error;
        ErrorMessages = errorMessages;
        Success = false;
    }
}