namespace Domain.Enums
{
    public enum ChangePasswordResult
    {
        Success,
        UserNotFound,
        IncorrectCurrentPassword,
        CurrentPasswordNotFound
    }
}
