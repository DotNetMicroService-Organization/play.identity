namespace Play.Identity.Services.Exceptions;

[Serializable]
internal class UnknownUserException : Exception
{
    public Guid userId { get; }

    public UnknownUserException(Guid userId) : base($"Unknown user '{userId}'")
    {
        this.userId = userId;
    }

}