namespace servartur.RealTimeUpdates;

public interface IConnectionMapping
{
    int Count { get; }

    void Add(int key, string connectionId);
    IEnumerable<string> GetConnections(int key);
    void Remove(int key, string connectionId);
    void RemoveAll(string connectionId);
}

//https://learn.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/mapping-users-to-connections#in-memory-storage
public class ConnectionMapping : IConnectionMapping
{
    private readonly Dictionary<int, HashSet<string>> _connections = [];

    public int Count
    {
        get
        {
            return _connections.Count;
        }
    }

    public void Add(int key, string connectionId)
    {
        lock (_connections)
        {
            HashSet<string> connections;
            if (!_connections.TryGetValue(key, out connections))
            {
                connections = new HashSet<string>();
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
        HashSet<string> connections;
        if (_connections.TryGetValue(key, out connections))
        {
            return connections;
        }

        return Enumerable.Empty<string>();
    }

    public void Remove(int key, string connectionId)
    {
        lock (_connections)
        {
            HashSet<string> connections;
            if (!_connections.TryGetValue(key, out connections))
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
    public void RemoveAll(string connectionId)
    {
        var keyValuePair = _connections.FirstOrDefault(pair => pair.Value.Contains(connectionId));
        int key = keyValuePair.Key;

        lock (_connections)
        {
            _connections.Remove(key);
        }
    }
}
