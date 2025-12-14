namespace servartur.Exceptions;

internal class ConflitedRequestDataException : Exception
{
    public ConflitedRequestDataException(string message)
        : base(message) { }
}
internal class RoomNotInMatchupException : ConflitedRequestDataException
{
    /// <param name="roomId">Id of the room that is not in matchup state.</param>
    public RoomNotInMatchupException(int roomId)
        : base($"Room with id {roomId} is not in matchup and this operation can't be done") { }
}
internal class RoomInBadStateException : ConflitedRequestDataException
{
    /// <param name="roomId">Id of the room that is not in bad state.</param>
    public RoomInBadStateException(int roomId)
        : base($"Room with id {roomId}'s state does not allow this operation ") { }
}

internal class RoomIsFullException : ConflitedRequestDataException
{
    /// <param name="roomId">Id of the room that is full.</param>
    public RoomIsFullException(int roomId)
        : base($"Room with id {roomId} is full and cannot be joined") { }
}

internal class SquadIsFullException : ConflitedRequestDataException
{
    /// <param name="roomId">Id of the room that's current squad is full.</param>
    public SquadIsFullException(int roomId)
        : base($"Current squad in room with id {roomId} is full") { }
}

internal class SquadIsEmptyException : ConflitedRequestDataException
{
    /// <param name="roomId">Id of the room that's current squad is empty.</param>
    public SquadIsEmptyException(int roomId)
        : base($"Current squad in room with id {roomId} is empty") { }
}

internal class SquadInWrongStateException : ConflitedRequestDataException
{
    /// <param name="squadId">Id of the squad that is in wrong state.</param>
    public SquadInWrongStateException(int squadId)
        : base($"Squad with id {squadId} is in wrong state for this operation") { }
}

internal class SquadIsNotFullException : ConflitedRequestDataException
{
    /// <param name="squadId">Id of the squad that is not full.</param>
    public SquadIsNotFullException(int squadId)
        : base($"Squad with id {squadId} is not filled and can't be submitted") { }
}

internal class PlayerHasAlreadyVotedException : ConflitedRequestDataException
{
    /// <param name="squadId">Id of the squad that ha ongoing voting.</param>
    public PlayerHasAlreadyVotedException(int squadId)
        : base($"Squad with id {squadId} already received vote from this player") { }
}


internal class TooManyEvilRolesException : ConflitedRequestDataException
{
    public TooManyEvilRolesException()
        : base("Too many evil roles for this number of players") { }
}
internal class PlayerCountInvalidException : ConflitedRequestDataException
{
    public PlayerCountInvalidException(int roomId)
        : base($"Room with id {roomId} has invalid number of players and game cannot be started") { }
}

internal class PercivalNotInGameException : ConflitedRequestDataException
{
    public PercivalNotInGameException(int roomId)
        : base($"Percival is not present in game of room {roomId}") { }
}
internal class PlayerIsNotAssassinException : ConflitedRequestDataException
{
    public PlayerIsNotAssassinException(int playerId)
        : base($"Player with id {playerId} is not an Assassin") { }
}
