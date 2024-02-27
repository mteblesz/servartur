namespace servartur.RealTimeUpdates;

public interface IConnectionMapping
{
    void Add(int key, string connectionId);
    IEnumerable<string> GetConnections(int key);
    void Remove(int key, string connectionId);
}

// https://learn.microsoft.com/pl-pl/aspnet/signalr/overview/guide-to-the-api/mapping-users-to-connections#in-memory-storage
public class ConnectionMapping : IConnectionMapping
{
    private readonly Dictionary<int, HashSet<string>> _connections = [];


    public void Add(int key, string connectionId)
    {
        lock (_connections)
        {
            if (!_connections.TryGetValue(key, out HashSet<string>? connections))
            {
                connections = [];
                _connections.Add(key, connections);
            }

            lock (connections)
            {
                connections.Add(connectionId);
            }
        }
    }

    public IEnumerable<string> GetConnections(int key)
    {
        if (_connections.TryGetValue(key, out HashSet<string>? connections))
        {
            return connections;
        }

        return Enumerable.Empty<string>();
    }

    public void Remove(int key, string connectionId)
    {
        lock (_connections)
        {
            if (!_connections.TryGetValue(key, out HashSet<string>? connections))
            {
                return;
            }

            lock (connections)
            {
                connections.Remove(connectionId);

                if (connections.Count == 0)
                {
                    _connections.Remove(key);
                }
            }
        }
    }
}