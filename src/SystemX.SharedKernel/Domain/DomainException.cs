namespace SystemX.SharedKernel.Domain;

/// <summary>Base exception for domain errors.</summary>
public class DomainException : Exception
{
    /// <summary>Creates the exception with a message.</summary>
    public DomainException(string message) : base(message)
    {
    }

    /// <summary>Creates the exception with a message and optional inner exception.</summary>
    public DomainException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    /// <summary>Creates a default domain exception.</summary>
    public DomainException() : base()
    {
    }
}
