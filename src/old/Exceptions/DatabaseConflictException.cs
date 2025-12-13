namespace servartur.Exceptions;

internal class DatabaseConflictException : Exception
{
    public DatabaseConflictException(string message)
        : base(message) { }
}

internal class PercivalButNoMerlinMorganaException : ConflitedRequestDataException
{
    /// <param name="roomId">Id of the room that is not in matchup state.</param>
    public PercivalButNoMerlinMorganaException(int roomId)
        : base($"Fatal Error: room with id {roomId} has Percival but Merlin/Morgana are not present.") { }
}
