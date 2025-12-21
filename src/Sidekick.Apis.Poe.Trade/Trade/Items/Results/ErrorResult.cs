namespace Sidekick.Apis.Poe.Trade.Trade.Items.Results;

public class ErrorResult
{
    public Error? Error { get; set; }
}

public class Error
{
    public ErrorResultCode Code { get; set; }
    public string? Message { get; set; }
}

public enum ErrorResultCode
{
    Accepted = 0,
    ResourceNotFound = 1,
    InvalidQuery = 2,
    RateLimitExceeded = 3,
    InternalError = 4,
    UnexpectedContentType = 5,
    Unauthorized = 8,
    Forbidden = 6,
    TemporarilyUnavailable = 7,
    MethodNotAllowed = 9,
    UnprocessableEntity = 10,
}
