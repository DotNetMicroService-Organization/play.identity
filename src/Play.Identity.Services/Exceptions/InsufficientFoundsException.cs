namespace Play.Identity.Services.Exceptions;

[Serializable]
internal class InsufficientFoundsException : Exception
{
    public Guid UserId { get; }
    public decimal GilToDeit { get; }
    public InsufficientFoundsException(Guid userId, decimal gilToDebit)
        : base($"Not enough gil to debit {gilToDebit} from user '{userId}'")
    {
        this.UserId = userId;
        this.GilToDeit = gilToDebit;
    }

}