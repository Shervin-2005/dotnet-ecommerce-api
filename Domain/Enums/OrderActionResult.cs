namespace Domain.Enums;

public enum OrderActionResult
{
    Success,
    NotFound,
    Forbidden,
    InvalidStatusTransition
}