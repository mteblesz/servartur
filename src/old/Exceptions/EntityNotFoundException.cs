namespace servartur.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entityName, int entityId)
        : base($"{entityName} with ID {entityId} does not exist.") { }
}
public class RoomNotFoundException : EntityNotFoundException
{
    /// <param name="roomId">Id of the room that was not found.</param>
    public RoomNotFoundException(int roomId)
        : base("Room", roomId) { }
}
public class PlayerNotFoundException : EntityNotFoundException
{
    /// <param name="playerId">Id of the player that was not found.</param>
    public PlayerNotFoundException(int playerId)
        : base("Player", playerId) { }
}
public class SquadNotFoundException : EntityNotFoundException
{
    /// <param name="squadId">Id of the squad that was not found.</param>
    public SquadNotFoundException(int squadId)
        : base("Squad", squadId) { }
}