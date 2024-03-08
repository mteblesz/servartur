namespace servartur.Exceptions;

public class ConflitedRequestDataException : Exception
{
    public ConflitedRequestDataException(string message) 
        : base(message) { }
}
public class RoomNotInMatchupException : ConflitedRequestDataException
{
    /// <param name="roomId">Id of the room that is not in matchup state.</param>
    public RoomNotInMatchupException(int roomId)
        : base($"Room with id {roomId} is not in matchup and cannot be joined") { }
}
public class RoomIsFullException : ConflitedRequestDataException
{
    /// <param name="roomId">Id of the room that is full.</param>
    public RoomIsFullException(int roomId)
        : base($"Room with id {roomId} is full and cannot be joined") { }
}

public class TooManyEvilRolesException : ConflitedRequestDataException
{
    public TooManyEvilRolesException()
        : base("Too many evil roles for this number of players") { }
}
public class PlayerCountInvalidException : ConflitedRequestDataException
{
    public PlayerCountInvalidException(int roomId)
        : base($"Room with id {roomId} has invalid number of players and game cannot be started") { }
}

public class PercivalNotInGameException : ConflitedRequestDataException
{
    public PercivalNotInGameException(int roomId)
        : base($"Percival is not present in game of room {roomId}") { }
}