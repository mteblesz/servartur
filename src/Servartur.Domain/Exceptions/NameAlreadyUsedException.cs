namespace Servartur.Domain.Exceptions;

public class NameAlreadyUsedException : Exception
{
    public NameAlreadyUsedException(string name, Guid roomId)
        : base($"Name '{name}' already taken in room {roomId}")
    {
    }
}
